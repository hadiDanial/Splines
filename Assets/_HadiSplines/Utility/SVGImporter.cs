using SVGImporter.Elements;
using SVGImporter.Elements.Containers;
using System.Collections.Generic;
using UnityEngine;
using MyVector2 = SVGImporter.Utility.Vector2;
using Vector2 = UnityEngine.Vector2;
using Rect = SVGImporter.Elements.Rect;
using SVGImporter.Elements.PathUtility;
using SVGImporter.Elements.Transforms;
using UnityEditor;
using static SVGImporter.Elements.PathUtility.SimpleMoveCommand;

namespace Hadi.Splines
{
    public class SVGImporter : MonoBehaviour
    {
        [SerializeField, Range(0.001f, 10), Tooltip("Scale from SVG to Unity units.")]
        private float svgToSplineScale = 0.01f;
        [SerializeField, Multiline(40)] private string svgCode;
        [SerializeField] private RendererSettings defaultRendererSettings;
        private Element importedElement;
        private TagType type;
        private Spline spline;
        private List<Point> points;

        public string SvgCode { get => svgCode; set => svgCode = value; }

        internal void SetElement(Element element)
        {
            this.importedElement = element;
            if (element == null) return;
            SetGameObjectName(element);
            
            Import();
        }

        private void SetGameObjectName(Element element)
        {
            string objectName = element.ElementId;
            if (string.IsNullOrEmpty(objectName)) objectName = element.ElementName;
            gameObject.name = objectName;
        }

        private void Import()
        {
            if (Element.IsContainer(importedElement.GetTagType()))
            {
                Transform parentTransform = transform;
                if (transform.childCount > 0)
                {
                    parentTransform = (new GameObject(importedElement.ElementName)).transform;
                    Undo.RegisterCreatedObjectUndo(parentTransform.gameObject, "Import SVG into new GameObject");
                    parentTransform.SetParent(transform.parent);
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                    transform.position = Vector3.zero;
                    transform.localScale = Vector3.one;
                }

                importedElement.Transform.ApplyTo(parentTransform, svgToSplineScale);

                ParentElement parent = (ParentElement)importedElement;
                foreach (var item in parent.Children)
                {
                    GameObject child = new GameObject(item.ElementName);
                    child.transform.SetParent(parentTransform);
                    SVGImporter childImporter = child.AddComponent<SVGImporter>();
                    childImporter.defaultRendererSettings = defaultRendererSettings;
                    childImporter.svgToSplineScale = svgToSplineScale;
                    childImporter.SetElement(item);
                }
            }
            else
            {
                type = importedElement.GetTagType();
                spline = gameObject.AddComponent<Spline>();
                transform.localPosition = Vector3.zero;
                spline.SplineData = new SplineData();
                spline.UseObjectTransform = true;
                spline.RendererSettings = defaultRendererSettings;
                spline.SplineData.settings = defaultRendererSettings;
                points = new List<Point>();
                switch (type)
                {
                    case TagType.Circle:
                        GenerateCircleSpline((Circle)importedElement);
                        break;
                    case TagType.Ellipse:
                        GenerateEllipseSpline((Ellipse)importedElement);
                        break;
                    case TagType.Line:
                        GenerateLine((Line)importedElement);
                        break;
                    case TagType.Path:
                        GeneratePath((Path)importedElement);
                        break;
                    case TagType.Polygon:
                        GeneratePolygon((Polygon)importedElement);
                        break;
                    case TagType.Polyline:
                        GeneratePolyline((Polyline)importedElement);
                        break;
                    case TagType.Rect:
                        GenerateRect((Rect)importedElement);
                        break;
                    default:
                        Debug.Log($"Incorrect type {type}");
                        break;
                }


                ApplyTransformations();

                spline.SetByElement(importedElement, points, defaultRendererSettings);
#if UNITY_EDITOR
                EditorApplication.delayCall += () => DestroyImmediate(this);
#endif
            }
        }

        private void ApplyTransformations()
        {
            foreach (TransformOperation operation in importedElement.Transform.Operations)
            {
                if (operation.IsAppliedToTransform())
                {
                    operation.ApplyTo(transform, svgToSplineScale);
                }
                else
                {
                    foreach (Point point in points)
                    {
                        point.anchor = importedElement.Transform.ApplyTo(point.anchor * svgToSplineScale);
                        point.relativeControlPoint1 = importedElement.Transform.ApplyTo(point.relativeControlPoint1 * svgToSplineScale);
                        point.relativeControlPoint2 = importedElement.Transform.ApplyTo(point.relativeControlPoint2 * svgToSplineScale);
                        point.Refresh(SplineMode.XY);
                    }
                }
            }
        }

        private void GeneratePath(Path element)
        {
            List<List<Point>> listOfPointLists = new List<List<Point>>();
            List<Point> currentPointList = new List<Point>();
            Vector2 currentPosition = Vector2.zero;
            Vector2 tempPos;
            foreach (var command in element.PathCommands)
            {
                switch (command)
                {
                    case ClosePathCommand:
                        spline.IsClosedSpline = true; 
                        break;
                    case MoveCommand moveCommand:
                        currentPointList = new List<Point>();
                        listOfPointLists.Add(currentPointList);
                        tempPos = MyVector2ToVector2(moveCommand.Point);
                        currentPosition = UpdateCurrentPosition(currentPosition, moveCommand.IsAbsolute, tempPos) * svgToSplineScale;
                        currentPointList.Add(new Point(currentPosition, Vector2.zero));
                        break;
                    case LineCommand lineCommand:
                        tempPos = MyVector2ToVector2(lineCommand.Point);
                        currentPosition = UpdateCurrentPosition(currentPosition, lineCommand.IsAbsolute, tempPos) * svgToSplineScale;
                        currentPointList.Add(new Point(currentPosition, Vector2.zero));
                        break;
                    case SimpleMoveCommand simpleMoveCommand:
                        tempPos = simpleMoveCommand.MovementType == SimpleMoveType.Horizontal ? new Vector2(simpleMoveCommand.Value, 0) : new Vector2(0, -simpleMoveCommand.Value);
                        currentPosition = UpdateCurrentPosition(currentPosition, simpleMoveCommand.IsAbsolute, tempPos) * svgToSplineScale; 
                        currentPointList.Add((new Point(currentPosition, Vector2.zero)));
                        break;
                    case ArcCommand arcCommand:
                        break;
                    case CubicCurveCommand ccCommand:
                        Vector2 cp1 = MyVector2ToVector2(ccCommand.ControlPoint1) * svgToSplineScale;
                        Vector2 cp2 = MyVector2ToVector2(ccCommand.ControlPoint2) * svgToSplineScale;
                        Vector2 position2 = MyVector2ToVector2(ccCommand.Point2) * svgToSplineScale;
                        Point point1 = currentPointList[currentPointList.Count - 1];
                        point1.relativeControlPoint2 = cp1 - (Vector2)point1.anchor;
                        //currentPointList.Add(new Point(currentPosition, cp1));
                        currentPosition = UpdateCurrentPosition(currentPosition, ccCommand.IsAbsolute, position2) * svgToSplineScale;
                        currentPointList.Add(new Point(currentPosition, cp2 - currentPosition));
                        break;
                    case CubicCurveContinueCommand ccContinueCommand:
                        currentPosition = UpdateCurrentPosition(currentPosition, ccContinueCommand.IsAbsolute, MyVector2ToVector2(ccContinueCommand.Point2)) * svgToSplineScale;
                        currentPointList.Add(new Point(currentPosition, MyVector2ToVector2(ccContinueCommand.ControlPoint2)));
                        break;
                    case QuadraticCurveCommand qcCommand:
                        break;
                    case QuadraticCurveContinueCommand qcContinueCommand:
                        break;
                    default:
                        break;
                }
            }
            points = currentPointList;
        }

        private void GeneratePolygon(Polygon element)
        {
            GeneratePolyline(element);
            spline.IsClosedSpline = true;
        }

        private void GenerateRect(Rect element)
        {
            //transform.localPosition = MyVector2ToVector2(element.Position) +Vector2.down * element.Size.y / 2 + Vector2.right * element.Position.x;
            Vector2 size = MyVector2ToVector2(element.Size) * svgToSplineScale;
            Vector2 position = MyVector2ToVector2(element.Position) * svgToSplineScale;
            transform.Translate(position, Space.Self);
            Vector2 origin = (position - Vector2.down * size.y / 2f + Vector2.right * size.x / 2f) - (Vector2)transform.localPosition;
            points = SplineShapesUtility.Rect(size, origin);
            spline.IsClosedSpline = true;
        }

        private void GeneratePolyline(Polyline element)
        {
            List<Vector2> linePoints = new List<Vector2>();
            var elementPoints = element.Points;
            Vector2 origin = MyVector2ToVector2(elementPoints[0]) * svgToSplineScale;
            transform.Translate(origin, Space.Self);
            linePoints.Add(Vector2.zero);
            for (int i = 1; i < elementPoints.Count; i++)
            {
                linePoints.Add(MyVector2ToVector2(elementPoints[i]) * svgToSplineScale - origin);
            }
            points = SplineShapesUtility.Polyline(linePoints);
        }

        private void GenerateLine(Line element)
        {
            //transform.localPosition = MyVector2ToVector2(center);
            Vector2 position = new Vector2(element.Point1.x, element.Point1.y) * svgToSplineScale;
            transform.Translate(position, Space.Self);
            points = SplineShapesUtility.Line(0, 0, element.Point2.x * svgToSplineScale, element.Point2.y * svgToSplineScale);
        }

        private void GenerateEllipseSpline(Ellipse element)
        {
            //transform.localPosition = MyVector2ToVector2(element.Center);
            transform.Translate(MyVector2ToVector2(element.Center) * svgToSplineScale, Space.Self);
            points = SplineShapesUtility.Ellipse(element.Radius.x * svgToSplineScale, element.Radius.y * svgToSplineScale);
            spline.IsClosedSpline = true;
            spline.UseObjectTransform = true;
        }

        private void GenerateCircleSpline(Circle element)
        {
            //transform.localPosition = MyVector2ToVector2(element.Center);
            transform.Translate(MyVector2ToVector2(element.Center) * svgToSplineScale, Space.Self);
            points = SplineShapesUtility.Circle(element.Radius * svgToSplineScale);
            spline.IsClosedSpline = true;
        }
        
        
        private static Vector2 UpdateCurrentPosition(Vector2 currentPos, bool isAbsolute, Vector2 newPosition)
        {
            if (isAbsolute)
                currentPos = newPosition;
            else
                currentPos += newPosition;
            return currentPos;
        }

        private static Vector2 MyVector2ToVector2(MyVector2 position)
        {
            return new Vector2(position.x, -position.y); // -y because SVG y-coordinates start from the top
        }

    }
}

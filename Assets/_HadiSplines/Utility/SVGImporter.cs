using SVGImporter.Elements;
using SVGImporter.Elements.Containers;
using System.Collections.Generic;
using UnityEngine;
using MyVector2 = SVGImporter.Utility.Vector2;
using Vector2 = UnityEngine.Vector2;
using Rect = SVGImporter.Elements.Rect;
using SVGImporter.Elements.PathUtility;
using static SVGImporter.Elements.PathUtility.SimpleMoveCommand;

namespace Hadi.Splines
{
    public class SVGImporter : MonoBehaviour
    {
        [SerializeField, Multiline(10)] private string svgCode;
        [SerializeField] private RendererSettings defaultRendererSettings;
        private Element element;
        private TagType type;
        private Spline spline;
        private List<Point> points;
        private bool useObjectTransform = false;

        public string SvgCode { get => svgCode; set => svgCode = value; }

        internal void SetElement(Element element)
        {
            this.element = element;
            if (element == null) return;
            SetGameObjectName(element);
            List<TagAttribute> attributes = element.Attributes;
            foreach (TagAttribute attr in attributes)
            {
                if(attr.attributeName.Equals("transform"))
                {
                    Debug.Log($"Found transform {attr.attributeValue}");
                    useObjectTransform = true;
                }
            }
            element.Transform.ApplyTo(transform);

            Import();
        }

        private void SetGameObjectName(Element element)
        {
            string name = element.ElementId;
            if (string.IsNullOrEmpty(name)) name = element.ElementName;
            gameObject.name = name;
        }

        private void Import()
        {
            if (Element.IsContainer(element.GetTagType()))
            {
                ParentElement parent = (ParentElement)element;
                foreach (var item in parent.Children)
                {
                    GameObject child = new GameObject(item.ElementName);
                    child.transform.SetParent(transform);
                    SVGImporter childImporter = child.AddComponent<SVGImporter>();
                    childImporter.defaultRendererSettings = defaultRendererSettings;
                    childImporter.SetElement(item);
                }
            }
            else
            {
                type = element.GetTagType();
                spline = gameObject.AddComponent<Spline>();
                spline.SplineData = new SplineData();
                spline.UseObjectTransform = useObjectTransform;
                spline.RendererSettings = defaultRendererSettings;
                spline.SplineData.settings = defaultRendererSettings;
                points = new List<Point>();
                switch (type)
                {
                    case TagType.Circle:
                        GenerateCircleSpline((Circle)element);
                        break;
                    case TagType.Ellipse:
                        GenerateEllipseSpline((Ellipse)element);
                        break;
                    case TagType.Line:
                        GenerateLine((Line)element);
                        break;
                    case TagType.Path:
                        GeneratePath((Path)element);
                        break;
                    case TagType.Polygon:
                        GeneratePolygon((Polygon)element);
                        break;
                    case TagType.Polyline:
                        GeneratePolyline((Polyline)element);
                        break;
                    case TagType.Rect:
                        GenerateRect((Rect)element);
                        break;
                    default:
                        Debug.Log($"Incorrect type {type}");
                        break;
                }
                spline.SetByElement(element, points, defaultRendererSettings);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(this);
#endif
            }
        }

        private void GeneratePath(Path element)
        {
            List<List<Point>> listOfPointLists = new List<List<Point>>();
            List<Point> currentPointList = new List<Point>();
            Vector2 currentPosition = Vector2.zero;
            Vector2 tempPos = Vector2.zero;
            foreach (var command in element.PathCommands)
            {
                switch (command)
                {
                    case ClosePathCommand closePath:
                        spline.IsClosedSpline = true; 
                        break;
                    case MoveCommand moveCommad:
                        currentPointList = new List<Point>();
                        listOfPointLists.Add(currentPointList);
                        tempPos = MyVector2ToVector2(moveCommad.Point);
                        currentPosition = UpdateCurrentPosition(currentPosition, moveCommad.IsAbsolute, tempPos);
                        currentPointList.Add(new Point(currentPosition, Vector2.zero));
                        break;
                    case LineCommand lineCommand:
                        tempPos = MyVector2ToVector2(lineCommand.Point);
                        currentPosition = UpdateCurrentPosition(currentPosition, lineCommand.IsAbsolute, tempPos);
                        points.Add(new Point(currentPosition, Vector2.zero));
                        break;
                    case SimpleMoveCommand simpleMoveCommad:
                        tempPos = simpleMoveCommad.MovementType == SimpleMoveType.Horizontal ? new Vector2(simpleMoveCommad.Value, 0) : new Vector2(0, -simpleMoveCommad.Value);
                        currentPosition = UpdateCurrentPosition(currentPosition, simpleMoveCommad.IsAbsolute, tempPos); 
                        points.Add((new Point(currentPosition, Vector2.zero)));
                        break;
                    case ArcCommand arcCommad:
                        break;
                    case CubicCurveCommand ccCommad:
                        break;
                    case CubicCurveContinueCommand ccContinueCommad:
                        break;
                    case QuadraticCurveCommand qcCommad:
                        break;
                    case QuadraticCurveContinueCommand qcContinueCommad:
                        break;
                    default:
                        break;
                }
            }
            points = currentPointList;
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

        private void GeneratePolygon(Polygon element)
        {
            GeneratePolyline(element);
            spline.IsClosedSpline = true;
        }

        private void GenerateRect(Rect element)
        {
            //transform.localPosition = MyVector2ToVector2(element.Position) +Vector2.down * element.Size.y / 2 + Vector2.right * element.Position.x;
            points = SplineShapesUtility.Rect(MyVector2ToVector2(element.Size), MyVector2ToVector2(element.Position));
            spline.IsClosedSpline = true;
            spline.UseObjectTransform = true;
        }

        private void GeneratePolyline(Polyline element)
        {
            List<Vector2> linePoints = new List<Vector2>();
            foreach (var item in element.Points)
            {
                linePoints.Add(MyVector2ToVector2(item));
            }
            points = SplineShapesUtility.Polyline(linePoints);
        }

        private void GenerateLine(Line element)
        {
            MyVector2 center = element.Point1 + element.Point2;
            center = center / 2;
            transform.localPosition = MyVector2ToVector2(center);
            points = SplineShapesUtility.Line(element.Point1.x, element.Point1.y, element.Point2.x, element.Point2.y);
        }

        private void GenerateEllipseSpline(Ellipse element)
        {
            transform.localPosition = MyVector2ToVector2(element.Center);
            points = SplineShapesUtility.Ellipse(element.Radius.x, element.Radius.y);
            spline.IsClosedSpline = true;
            spline.UseObjectTransform = true;
        }

        private void GenerateCircleSpline(Circle element)
        {
            transform.localPosition = MyVector2ToVector2(element.Center);
            points = SplineShapesUtility.Circle(element.Radius);
            spline.IsClosedSpline = true;
            spline.UseObjectTransform = true;
        }
    }
}

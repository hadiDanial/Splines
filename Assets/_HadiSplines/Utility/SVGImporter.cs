using SVGImporter.Elements;
using SVGImporter.Elements.Containers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGImporter.Utility;
using MyVector2 = SVGImporter.Utility.Vector2;
using Vector2 = UnityEngine.Vector2;
using Rect = SVGImporter.Elements.Rect;
using System.Numerics;

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
            foreach (var command in element.PathCommands)
            {
                
            }
        }

        private void GeneratePolygon(Polygon element)
        {
            GeneratePolyline(element);
            spline.IsClosedSpline = true;
        }

        private void GenerateRect(Rect element)
        {
            transform.localPosition = new Vector2(element.Position.x, element.Position.y);
            points = SplineShapesUtility.Rect(new Vector2(element.Size.x, element.Size.y));
            spline.IsClosedSpline = true;
        }

        private void GeneratePolyline(Polyline element)
        {
            List<Vector2> linePoints = new List<Vector2>();
            foreach (var item in element.Points)
            {
                linePoints.Add(new Vector2(item.x, item.y));
            }
            points = SplineShapesUtility.Polyline(linePoints);
        }

        private void GenerateLine(Line element)
        {
            MyVector2 center = element.Point1 + element.Point2;
            center = center / 2;
            transform.localPosition = new Vector2(center.x, center.y);
            points = SplineShapesUtility.Line(element.Point1.x, element.Point1.y, element.Point2.x, element.Point2.y);
        }

        private void GenerateEllipseSpline(Ellipse element)
        {
            transform.localPosition = new Vector2(element.Center.x, element.Center.y);
            points = SplineShapesUtility.Ellipse(element.Radius.x, element.Radius.y);
            spline.IsClosedSpline = true;
        }

        private void GenerateCircleSpline(Circle element)
        {
            transform.localPosition = new Vector2(element.Center.x, element.Center.y);
            points = SplineShapesUtility.Circle(element.Radius);
            spline.IsClosedSpline = true;
        }
    }
}

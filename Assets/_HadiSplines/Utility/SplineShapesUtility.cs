using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public class SplineShapesUtility 
    {
        public static List<Point> CreateShape(SplineShapes shape, bool closedSpline)
        {
            switch (shape)
            {
                case SplineShapes.DefaultShape:
                    return DefaultSpline(closedSpline);
                case SplineShapes.Circle:
                    return Circle();
                case SplineShapes.Square:
                    return Square();
                case SplineShapes.Rect:
                    return Rect();
                case SplineShapes.Line:
                    return Line(-1, 0, 1, 0);
                default:
                    return DefaultSpline(closedSpline);
            }
        }


        private static List<Point> Square()
        {
            return Square(2);
        }

        public static List<Point> Square(float length)
        {
            return Rect(new Vector2(length, length));
        }

        private static List<Point> Rect()
        {
            return Rect(new Vector2(4, 2));
        }

        public static List<Point> Rect(Vector2 size)
        {
            return Rect(size, Vector2.zero);
        }

        public static List<Point> Rect(Vector2 size, Vector2 origin)
        {
            List<Point> points = new List<Point>();
            float w = size.x / 2, h = size.y / 2;
            Vector2 tr = new Vector2(w, h) + origin;
            Vector2 br = new Vector2(w, -h) + origin;
            Vector2 bl = new Vector2(-w, -h) + origin;
            Vector2 tl = new Vector2(-w, h) + origin;
            Point P1 = new Point(tr, Vector2.zero);
            P1.rotation = Quaternion.Euler(0, 0, -45);
            Point P2 = new Point(br, Vector2.zero);
            P2.rotation = Quaternion.Euler(0, 0, -135); 
            Point P3 = new Point(bl, Vector2.zero);
            P3.rotation = Quaternion.Euler(0, 0, 135); 
            Point P4 = new Point(tl, Vector2.zero);
            P4.rotation = Quaternion.Euler(0, 0, 45); 

            points.Add(P1);
            points.Add(P2);
            points.Add(P3);
            points.Add(P4);
            return points;
        }
        private static List<Point> Circle()
        {
            return Circle(1);
        }
        
        public static List<Point> Circle(float r)
        {
            return Ellipse(r, r);
        }

        public static List<Point> Ellipse(float rx, float ry)
        {
            List<Point> points = new List<Point>();
            // Values taken from https://spencermortensen.com/articles/bezier-circle/
            float a = 1.00005519f, b = 0.55342686f, c = 0.99873585f;
            Vector3 tangent1 = new Vector2(b, c-a) * rx;
            Vector3 tangent2 = new Vector2(c-a, b) * ry;
            Point P1 = new Point(new Vector2(0, a) * ry, -tangent1);
            Point P2 = new Point(new Vector2(a, 0) * rx, tangent2);
            P2.rotation = Quaternion.Euler(0, 0, -90);
            Point P3 = new Point(new Vector2(0, -a) * ry, tangent1);
            P3.rotation = Quaternion.Euler(0, 0, -180); 
            Point P4 = new Point(new Vector2(-a, 0) * rx, -tangent2);
            P4.rotation = Quaternion.Euler(0, 0, 90);
            points.Add(P1);
            points.Add(P2);
            points.Add(P3);
            points.Add(P4);
            return points;
        }

     
        public static List<Point> Line(float x1, float y1, float x2, float y2)
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(new Vector2(x1, y1), Vector2.zero));
            points.Add(new Point(new Vector2(x2, y2), Vector2.zero));
            return points;
        }

        public static List<Point> Polyline(List<Vector2> linePoints)
        {
            List<Point> points = new List<Point>();
            foreach (var item in linePoints)
            {
                points.Add(new Point(item, Vector2.zero));
            }
            return points;
        }

        private static List<Point> DefaultSpline(bool closedSpline)
        {
            List<Point> points = new List<Point>();

            Vector3 control = (Vector3.left + Vector3.up) * 0.5f;
            Point p1 = new Point(Vector3.left, control);
            Point p2 = new Point(Vector3.right, control);

            points.Add(p1);
            points.Add(p2);
            if (closedSpline)
            {
                Point p3 = new Point(Vector3.down, Vector3.right);
                p3.rotation = Quaternion.Euler(180, 0, 0);
                points.Add(p3);
            }
            return points;
        }

    }

    public enum SplineShapes
    {
        DefaultShape,
        Circle,
        Square,
        Rect,
        Line
    }
}

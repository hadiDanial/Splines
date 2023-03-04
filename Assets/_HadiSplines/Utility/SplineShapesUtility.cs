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
                default:
                    return DefaultSpline(closedSpline);
            }
        }

        private static List<Point> Square()
        {
            List<Point> points = new List<Point>();
            Vector2 tr = new Vector2(1, 1);
            Vector2 br = new Vector2(1, -1);
            Vector2 bl = new Vector2(-1, -1);
            Vector2 tl = new Vector2(-1, 1);
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
            List<Point> points = new List<Point>();
            // Values taken from https://spencermortensen.com/articles/bezier-circle/
            float a = 1.00005519f, b = 0.55342686f, c = 0.99873585f;
            Vector3 tangent1 = new Vector2(b, c-a);
            Vector3 tangent2 = new Vector2(c-a, b);
            Point P1 = new Point(new Vector2(0, a), -tangent1);
            Point P2 = new Point(new Vector2(a, 0), tangent2);
            P2.rotation = Quaternion.Euler(0, 0, -90);
            Point P3 = new Point(new Vector2(0, -a), tangent1);
            P3.rotation = Quaternion.Euler(0, 0, -180); 
            Point P4 = new Point(new Vector2(-a, 0), -tangent2);
            P4.rotation = Quaternion.Euler(0, 0, 90);
            points.Add(P1);
            points.Add(P2);
            points.Add(P3);
            points.Add(P4);
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
        Square
    }
}

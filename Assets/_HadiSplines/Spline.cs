using Codice.Client.BaseCommands.BranchExplorer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public class Spline : MonoBehaviour
    {
        [SerializeField]
        protected bool closedSpline = false;
        [SerializeField, Range(1, 100)]
        protected int segmentsPerCurve = 10;
        [SerializeField]
        protected List<Vector3> segmentedPoints;
        [SerializeField]
        protected List<Transform> points;
        [SerializeField]
        private SplineRendererType rendererType = SplineRendererType.LineRenderer;
        [SerializeField]
        private List<Curve> curves;
        [Header("DEBUG")]
        [SerializeField]
        private bool drawGizmos = true;


        private ISplineRenderer splineRenderer;

        /// <summary>
        /// Number of points per curve (two anchors, and a control point for each anchor).
        /// </summary>
        private const int POINT_COUNT_PER_CURVE = 4;

        private const float ANCHOR_SIZE = 0.01f, CONTROL_SIZE = 0.01f;

        private void Awake()
        {
            SetRendererType();
            curves = new List<Curve>();
            segmentedPoints = new List<Vector3>();
        }

        private void Start()
        {
            GenerateCurve();
        }

        private void GenerateCurve()
        {
            Point p1 = new Point(new Vector3(-2, -1), Vector3.left * 5, Vector3.right + Vector3.one * 0.25f);
            Point p2 = new Point(Vector3.one, Vector3.left, Vector3.right);
            Point p3 = new Point(Vector3.zero, new Vector3(-0.5f, -1));
            Point p4 = new Point(Vector3.one, new Vector2(-0.3f, 0.75f), 1f, 0.75f);
            Point p5 = new Point(new Vector3(3, -4), new Vector2(0, 0.75f), 1f, 0.75f);
            Curve curve1 = new Curve { P1 = p1, P2 = p2 };
            Curve curve2 = new Curve { P1 = p2, P2 = p5 };
            curves.Add(curve1);
            curves.Add(curve2);
        }

        protected void SetRendererType()
        {
            switch (rendererType)
            {
                case SplineRendererType.LineRenderer:
                    splineRenderer = gameObject.AddComponent<SplineLineRenderer>();
                    break;
                case SplineRendererType.MeshRenderer:
                    break;
                case SplineRendererType.QuadRenderer:
                    break;
                default:
                    splineRenderer = gameObject.AddComponent<SplineLineRenderer>();
                    break;
            }
        }

        void Update()
        {
            SetupSplinePoints();
        }

        /// <summary>
        /// Setup the spline based on the curves list.
        /// </summary>
        [ContextMenu("Set Points")]
        protected virtual void SetupSplinePoints()
        {
            int curvesCount = curves.Count;
            if (splineRenderer == null || curvesCount == 0) return;
            int pointsPerCurve = segmentsPerCurve * POINT_COUNT_PER_CURVE;
            int numPositions = curvesCount * pointsPerCurve;//points.Count * segmentsPerCurve;
            splineRenderer.SetPointCount(closedSpline ? numPositions + pointsPerCurve : numPositions);


            segmentedPoints.Clear();
            for (int i = 0; i < curvesCount; i++)
            {
                CalculateSegmentedPoints(curves[i], i);
            }
            if (closedSpline)
                CloseSpline();

            splineRenderer.SetPoints(segmentedPoints);
        }

        /// <summary>
        /// Close the spline by connecting the last point with the first point.
        /// </summary>
        protected virtual void CloseSpline()
        {
            if (curves.Count < 2)
            {
                Debug.LogError("Cannot close a spline with less than two curves! " + gameObject.name);
                return;
            }

            Curve initCurve = curves[0], finalCurve = curves[curves.Count - 1];
            Curve c = new Curve(finalCurve.P2, initCurve.P1);
            CalculateSegmentedPoints(c, curves.Count);
        }

        /// <summary>
        /// Calculates a cubic bezier using polynomial coefficients, split into segments, and saves the results in <code>segmentedPoints.</code>
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="index"></param>
        protected virtual void CalculateSegmentedPoints(Curve curve, int index)
        {
            int totalSegments = segmentsPerCurve * POINT_COUNT_PER_CURVE;
            Vector3 factor0 = curve.P1.anchor, factor1 = -3 * curve.P1.anchor + 3 * curve.P1.controlPoint2;
            Vector3 factor2 = 3 * curve.P1.anchor - 6 * curve.P1.controlPoint2 + 3 * curve.P2.controlPoint1;
            Vector3 factor3 = -curve.P1.anchor + 3 * curve.P1.controlPoint2 - 3 * curve.P2.controlPoint1 + curve.P2.anchor;
            int start = index * segmentsPerCurve * POINT_COUNT_PER_CURVE, end = start + totalSegments;
            for (int i = start; i < end; i++)
            {
                float t = ((float)i % totalSegments) / (end - start);
                print("t=" + t + ", start=" + start + ", end=" + end);
                float t2 = t * t;
                float t3 = t2 * t;
                Vector3 P = factor0 + t * factor1 + t2 * factor2 + t3 * factor3;
                segmentedPoints.Add(P);
            }
        }

        /// <summary>
        /// Calculates a cubic bezier using DeCasteljau's method
        /// </summary>
        /// <param name="p0">Control Point of first anchor</param>
        /// <param name="p1">Anchor 1</param>
        /// <param name="p2">Control Point of second anchor</param>
        /// <param name="p3">Anchor 2</param>
        /// <param name="t">Time</param>
        /// <returns></returns>
        protected virtual Vector3 CalculatePointPosition(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 a, b, c, d, e;
            a = Vector3.Lerp(p0, p1, t);
            b = Vector3.Lerp(p1, p2, t);
            c = Vector3.Lerp(p2, p3, t);
            d = Vector3.Lerp(a, b, t);
            e = Vector3.Lerp(b, c, t);
            return Vector3.Lerp(d, e, t);
        }

        private void OnDrawGizmos()
        {
            if (curves.Count == 0 || !drawGizmos) return;
            foreach (Curve curve in curves)
            {
                if (curve.Equals(default(Curve)))
                    continue;

                Point[] points = { curve.P1, curve.P2 };
                foreach (Point point in points)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(point.anchor, point.controlPoint1);
                    Gizmos.DrawLine(point.anchor, point.controlPoint2);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(point.anchor, ANCHOR_SIZE);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(point.controlPoint1, CONTROL_SIZE);
                    Gizmos.DrawSphere(point.controlPoint2, CONTROL_SIZE);
                }
            }
        }


        //private void CalculateSplineBasedOnTransforms(int numPositions)
        //{
        //    int totalSegments = segmentsPerCurve * POINTS_PER_CURVE;

        //    // Every four points make up a cubic curve
        //    for (int j = 0; j < points.Count; j += POINTS_PER_CURVE)
        //    {
        //        // By caching these, we reduce the amount of computations needed. Same computation as in CalculatePointPosition, but more efficient.
        //        // See `The Continuity of Splines` by Freya Holmer, @6:10
        //        Vector3 factor0 = points[j].position, factor1 = -3 * points[j].position + 3 * points[j + 1].position;
        //        Vector3 factor2 = 3 * points[j].position - 6 * points[j + 1].position + 3 * points[j + 2].position;
        //        Vector3 factor3 = -points[j].position + 3 * points[j + 1].position - 3 * points[j + 2].position + points[j + 3].position;
        //        int start = j * segmentsPerCurve, end = start + totalSegments;
        //        for (int i = start; i < end; i++)
        //        {
        //            float t = ((float)i % totalSegments) / (end - start);
        //            print("t=" + t + ", start=" + start + ", end=" + end);
        //            float t2 = t * t;
        //            float t3 = t2 * t;
        //            Vector3 P = factor0 + t * factor1 + t2 * factor2 + t3 * factor3;
        //            //CalculatePointPosition(points[j].position, points[j + 1].position, points[j + 2].position, points[j + 3].position, t)
        //            splineRenderer.SetPoint(i, P);

        //        }
        //    }
        //}
    }
}

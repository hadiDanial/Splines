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
        protected List<Point> points;
        [SerializeField]
        protected List<Vector3> segmentedPoints, normals, tangents;
        [SerializeField]
        private SplineRendererType rendererType = SplineRendererType.LineRenderer;
        [Header("DEBUG")]
        [SerializeField]
        private bool drawGizmos = true;
        [SerializeField]
        private bool drawNormals, drawTangents;
        [SerializeField, Range(0.01f, 0.5f)]
        private float controlSize = 0.2f;
        [Range(0.01F, 0.5F), SerializeField]
        private float anchorSize = 0.15f;


        private ISplineRenderer splineRenderer;

        /// <summary>
        /// Number of points per curve (two anchors, and a control point for each anchor).
        /// </summary>
        private const int POINT_COUNT_PER_CURVE = 4;

        public float ANCHOR_SIZE { get => anchorSize; }
        public float CONTROL_SIZE { get => controlSize; }

        private void Awake()
        {
            SetRendererType();
            points = new List<Point>();
            segmentedPoints = new List<Vector3>();
            normals = new List<Vector3>();
            tangents = new List<Vector3>();
        }

        private void Start()
        {
            GenerateCurve();
        }

        [ContextMenu("Add Point")]
        public void AddPoint()
        {
            points.Add(new Point(Vector3.zero, Vector3.left, Vector3.right));
        }
        private void GenerateCurve()
        {
            Point p1 = new Point(new Vector3(-2, -1), Vector3.left * 5, Vector3.right + Vector3.one * 0.25f);
            Point p2 = new Point(Vector3.one, Vector3.left, Vector3.right);
            Point p3 = new Point(Vector3.zero, new Vector3(-0.5f, -1));
            Point p4 = new Point(Vector3.one, new Vector2(-0.3f, 0.75f), 1f, 0.75f);
            Point p5 = new Point(new Vector3(3, -4), new Vector2(0, 0.75f), 1f, 0.75f);
            Point p6 = new Point(Vector3.zero, Vector3.left, Vector3.right);
            Point p7 = new Point(Vector3.right + Vector3.up, Vector3.left * 1.3f, Vector3.zero);
            //points.Add(p1);
            //points.Add(p2);
            //points.Add(p3);
            //points.Add(p4);
            points.Add(p6);
            points.Add(p7);
            points.Add(p5);
        }

        [ContextMenu("Setup Renderer")]
        protected void SetRendererType()
        {
            splineRenderer = GetComponent<ISplineRenderer>();
            if (splineRenderer != null)
                return;
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
            splineRenderer.Setup();
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
            int pointsCount = points.Count;
            if (splineRenderer == null)
            {
                SetRendererType();
                Debug.LogWarning("Cannot create a spline without a renderer! Renderer added automatically." + gameObject.name);
                return;
            }
            if (pointsCount < 2)
            {
                Debug.LogError("Cannot create a spline without at least 2 points! " + gameObject.name);
                return;
            }
            int pointsPerCurve = segmentsPerCurve * POINT_COUNT_PER_CURVE;
            int numPositions = pointsCount * POINT_COUNT_PER_CURVE * (pointsPerCurve + 1);//points.Count * segmentsPerCurve;
            splineRenderer.SetPointCount(closedSpline ? numPositions + pointsPerCurve : numPositions);


            segmentedPoints.Clear();
            tangents.Clear();
            normals.Clear();
            for (int i = 0; i < pointsCount - 1; i++)
            {
                CalculateSegmentedPoints(points[i], points[i + 1], i);
            }
            if (closedSpline)
            {
                splineRenderer.SetClosedShape(true);
                CloseSpline();
            }
            else
                splineRenderer.SetClosedShape(false);

            splineRenderer.SetPoints(segmentedPoints);
        }

        /// <summary>
        /// Close the spline by connecting the last point with the first point.
        /// </summary>
        protected virtual void CloseSpline()
        {
            if (points.Count < 3)
            {
                Debug.LogError("Cannot close a spline with less than three points! " + gameObject.name);
                closedSpline = false;
                return;
            }
            CalculateSegmentedPoints(points[points.Count - 1], points[0], points.Count - 1);
        }

        /// <summary>
        /// Calculates a cubic bezier using polynomial coefficients, split into segments, and saves the results in <code>segmentedPoints.</code>
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="index"></param>
        protected virtual void CalculateSegmentedPoints(Point P1, Point P2, int index)
        {
            int totalSegments = segmentsPerCurve * POINT_COUNT_PER_CURVE;

            // By caching these, we reduce the amount of computations needed. Same result as DeCasteljau's, but more efficient.
            // See `The Continuity of Splines` by Freya Holmer, @6:10
            Vector3 factor0 = P1.anchor, factor1 = -3 * P1.anchor + 3 * P1.controlPoint2;
            Vector3 factor2 = 3 * P1.anchor - 6 * P1.controlPoint2 + 3 * P2.controlPoint1;
            Vector3 factor3 = -P1.anchor + 3 * P1.controlPoint2 - 3 * P2.controlPoint1 + P2.anchor;
            int start = index * segmentsPerCurve * POINT_COUNT_PER_CURVE, end = start + totalSegments;
            for (int i = start; i < end; i++)
            {
                float t = ((float)i % totalSegments) / (end - start);
                //print("t=" + t + ", start=" + start + ", end=" + end);
                float t2 = t * t;
                float t3 = t2 * t;
                Vector3 P = factor0 + t * factor1 + t2 * factor2 + t3 * factor3;
                segmentedPoints.Add(P);
                P = factor1 + 2 * t * factor2 + 3 * t2 * factor3;
                tangents.Add(P);
                P = Vector3.Cross(Quaternion.Lerp(P1.anchorRotation, P2.anchorRotation, t).eulerAngles, P); // Cross the tangent vector with the normal of the two points (lerped)
                normals.Add(P.normalized);
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
        protected virtual Vector3 DeCasteljau(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 a, b, c, d, e;
            a = Vector3.Lerp(p0, p1, t);
            b = Vector3.Lerp(p1, p2, t);
            c = Vector3.Lerp(p2, p3, t);
            d = Vector3.Lerp(a, b, t);
            e = Vector3.Lerp(b, c, t);
            return Vector3.Lerp(d, e, t);
        }

        protected void OnValidate()
        {
            foreach (Point point in points)
            {
                point.Refresh();
            }
        }

        private void OnDrawGizmos()
        {
            if (points.Count == 0 || !drawGizmos) return;
            if (normals.Count == 0 || !drawNormals) return;
            Gizmos.color = Color.red;
            for (int i = 0; i < normals.Count; i++)
            {
                Gizmos.DrawRay(segmentedPoints[i], normals[i] * 0.15f);
            }
            Gizmos.color = Color.green;
            if (tangents.Count == 0 || !drawTangents) return;
            for (int i = 0; i < tangents.Count; i++)
            {
                Gizmos.DrawRay(segmentedPoints[i], tangents[i] * 0.15f);
            }
        }

        public List<Point> GetPoints()
        {
            return points;
        }
    }
}

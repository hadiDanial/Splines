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
        protected List<Transform> points;
        [SerializeField]
        private SplineRendererType rendererType = SplineRendererType.LineRenderer;

        private ISplineRenderer splineRenderer;
        private const int POINTS_PER_CURVE = 4;


        private void Awake()
        {
            SetRendererType();
        }

        protected  void SetRendererType()
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

        [ContextMenu("Set Points")]
        protected virtual void SetupSplinePoints()
        {
            if (splineRenderer == null) return;
            int numPositions = points.Count * segmentsPerCurve;
            int totalSegments = segmentsPerCurve * POINTS_PER_CURVE;
            splineRenderer.SetPointCount(closedSpline ? numPositions + segmentsPerCurve : numPositions);

            // Every four points make up a cubic curve
            for (int j = 0; j < points.Count; j += POINTS_PER_CURVE)
            {
                // By caching these, we reduce the amount of computations needed. Same computation as in CalculatePointPosition, but more efficient.
                // See `The Continuity of Splines` by Freya Holmer, @6:10
                Vector3 factor0 = points[j].position, factor1 = -3 * points[j].position + 3 * points[j + 1].position;
                Vector3 factor2 = 3 * points[j].position - 6 * points[j + 1].position + 3 * points[j + 2].position;
                Vector3 factor3 = -points[j].position + 3 * points[j + 1].position - 3 * points[j + 2].position + points[j + 3].position;
                int start = j * segmentsPerCurve, end = start + totalSegments;
                for (int i = start; i < end; i++)
                {
                    float t = ((float)i%totalSegments) / (end - start);
                    print("t=" + t + ", start=" + start + ", end=" + end);
                    float t2 = t * t;
                    float t3 = t2 * t;
                    Vector3 P = factor0 + t * factor1 + t2 * factor2 + t3 * factor3;
                    //CalculatePointPosition(points[j].position, points[j + 1].position, points[j + 2].position, points[j + 3].position, t)
                    splineRenderer.SetPoint(i, P);

                }
            }
            if (closedSpline)
            {
                int j = points.Count - 2;
                for (int i = numPositions; i < numPositions + segmentsPerCurve; i++)
                {
                    float t = ((float)i - numPositions) / segmentsPerCurve;
                    splineRenderer.SetPoint(i, CalculatePointPosition(points[j - 1].position, points[j].position, points[0].position, points[1].position, t));
                }
            }
        }


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
    }
}

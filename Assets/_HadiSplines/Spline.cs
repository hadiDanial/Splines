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
        private int segments = 10;
        [SerializeField]
        private List<Transform> points;
        [SerializeField]
        private SplineRendererType rendererType = SplineRendererType.LineRenderer;
        
        private ISplineRenderer splineRenderer;

        private void Awake()
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

        void Start()
        {
            SetupSplinePoints();
        }

        [ContextMenu("Set Points")]
        private void SetupSplinePoints()
        {
            if (splineRenderer == null) return;
            int numPositions = points.Count * segments;
            splineRenderer.SetPointCount(closedSpline ? numPositions + segments : numPositions);
            
            // Every four points make up a cubic curve
            for (int j = 0; j < points.Count; j += 4)
            {
                for (int i = 0; i < numPositions; i++)
                {
                    float t = ((float)i) / numPositions;
                    splineRenderer.SetPoint(i, CalculatePointPosition(points[j].position, points[j + 1].position, points[j + 2].position, points[j + 3].position, t));
                    
                }
            }
            if (closedSpline)
            {
                int j = points.Count - 3;
                for (int i = numPositions; i < numPositions + segments; i++)
                {
                    float t = ((float)i - numPositions) / segments;
                    splineRenderer.SetPoint(i, CalculatePointPosition(points[j - 1].position, points[j].position, points[j + 1].position, points[0].position, t));
                }
            }
        }
        [ExecuteInEditMode]
        void OnValidate()
        {
            SetupSplinePoints();
        }


        private Vector3 CalculatePointPosition(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
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

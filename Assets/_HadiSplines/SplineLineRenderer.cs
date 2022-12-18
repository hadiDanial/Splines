using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [RequireComponent(typeof(LineRenderer))]
    internal class SplineLineRenderer : MonoBehaviour, ISplineRenderer
    {
        private LineRenderer lineRenderer;

        private void Awake()
        {
            Setup();
        }
        public void AddPoint(Vector3 point)
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);
        }

        public void SetPointCount(int count)
        {
            lineRenderer.positionCount = count;
        }

        public void SetPoint(int index, Vector3 point)
        {
            lineRenderer.SetPosition(index, point);
        }

        public void SetPoints(List<Vector3> points)
        {
            SetPointCount(points.Count);
            for (int i = 0; i < points.Count; i++)            
            {
                SetPoint(i, points[i]);
            }
        }

        public void Clear()
        {
            this.lineRenderer.positionCount = 0;
        }

        public void Setup()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.2f;
        }

        public void SetClosedShape(bool closed)
        {
            lineRenderer.loop = closed;
        }
    }
}
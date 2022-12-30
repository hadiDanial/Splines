using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [RequireComponent(typeof(LineRenderer))]
    internal class SplineLineRenderer : MonoBehaviour, ISplineRenderer
    {
        private LineRenderer lineRenderer;
        private SplineData splineData;

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

        public void SetData(SplineData splineData)
        {
            this.splineData = splineData;
            int count = splineData.SegmentedPoints.Count;
            SetPointCount(count);
            for (int i = 0; i < count; i++)            
            {
                SetPoint(i, splineData.SegmentedPoints[i]);
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
            lineRenderer.startWidth = 0.05f;
        }

        public void SetClosedShape(bool closed)
        {
            lineRenderer.loop = closed;
        }
    }
}
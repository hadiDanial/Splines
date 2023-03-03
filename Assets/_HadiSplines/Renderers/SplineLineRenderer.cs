using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [ExecuteInEditMode]
    internal class SplineLineRenderer : MonoBehaviour, ISplineRenderer
    {
        private LineRenderer lineRenderer;
        private SplineData splineData;
        private void Awake()
        {
            InitializeLineRenderer();            
        }

        public void Setup(RendererSettings settings)
        {
            InitializeLineRenderer();
            lineRenderer.material = settings.Material;
        }

        private void InitializeLineRenderer()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
        }


        public void SetData(SplineData splineData)
        {
            this.splineData = splineData;
            int count = splineData.Points.Count;
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = count;
            for (int i = 0; i < splineData.Points.Count; i++)
            {
                lineRenderer.SetPosition(i, splineData.Points[i]);
            }
        }
        public void SetClosedSpline(bool closed)
        {
            lineRenderer.loop = closed;
        }

        public void SetFill(bool isFilled = true)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            this.lineRenderer.positionCount = 0;
        }

        public void Destroy()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if(lineRenderer != null)
                    DestroyImmediate(lineRenderer);
                DestroyImmediate(this);
            };
#endif
        }
        public SplineRendererType GetRendererType()
        {
            return SplineRendererType.LineRenderer;
        }

    }
}
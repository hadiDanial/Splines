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

        public void Setup(Material material)
        {
            InitializeLineRenderer();
            lineRenderer.material = material;
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
            lineRenderer.positionCount = count;
            for (int i = 0; i < splineData.Points.Count; i++)
            {
                lineRenderer.SetPosition(i, transform.TransformSplinePoint(splineData.Points[i], splineData.useWorldSpace));
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
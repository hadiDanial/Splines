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
        private SplineLineRendererSettings settings;
        private void Awake()
        {
            InitializeLineRenderer();            
        }

        public void Setup()
        {
            InitializeLineRenderer();
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
            settings = (SplineLineRendererSettings)splineData.settings;
            lineRenderer.material = splineData.settings?.Material;
            int count = splineData.SegmentedPoints.Count;
            
            if (settings.useWidthCurve)
            {
                lineRenderer.widthCurve = settings.WidthCurve;
            }
            else
            {
                lineRenderer.widthCurve = AnimationCurve.Constant(0,1,settings.StartWidth);
                lineRenderer.startWidth = settings.StartWidth;
                lineRenderer.endWidth = settings.EndWidth;
            }
            lineRenderer.startColor = settings.StartColor;
            lineRenderer.endColor = settings.EndColor;
            lineRenderer.alignment = settings.LineAlignment;

            lineRenderer.positionCount = count;
            for (int i = 0; i < splineData.SegmentedPoints.Count; i++)
            {
                lineRenderer.SetPosition(i, splineData.SegmentedPoints[i]);
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
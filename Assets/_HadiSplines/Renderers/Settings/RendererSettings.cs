using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public abstract class RendererSettings : ScriptableObject
    {
        [SerializeField] private Material material;
        [SerializeField] private UVGenerationType uVGenerationType;
        protected SplineRendererType rendererType;
        public UVGenerationType UVGenerationType { get => uVGenerationType; }
        public Material Material { get => material; }
        public SplineRendererType RendererType { get => rendererType; }

        private void OnEnable()
        {
            SetRendererType();
        }

        protected abstract void SetRendererType();
        

        public static ISplineRenderer GetRenderer(RendererSettings settings, GameObject splineObject, SplineData splineData)
        {
            ISplineRenderer splineRenderer = null;
            switch (settings.rendererType)
            {
                case SplineRendererType.LineRenderer:
                    splineRenderer = GetNewRenderer<SplineLineRenderer>(splineObject);
                    break;
                case SplineRendererType.TubeMeshRenderer:
                    splineRenderer = GetNewRenderer<TubeMeshRenderer>(splineObject);
                    break;
                case SplineRendererType.CustomMeshRenderer:
                    splineRenderer = GetNewRenderer<CustomMeshRenderer>(splineObject);
                    break;
                case SplineRendererType.None:
                    break;
                default:
                    splineRenderer = GetNewRenderer<SplineLineRenderer>(splineObject);
                    break;
            }
            splineRenderer?.Setup();
            splineRenderer?.SetData(splineData);
            return splineRenderer;
        }
        private static ISplineRenderer GetNewRenderer<T>(GameObject splineObject) where T : MonoBehaviour, ISplineRenderer
        {
            ISplineRenderer renderer = splineObject.GetComponent<T>();
            if (renderer == null)
                renderer = splineObject.AddComponent<T>();
            return renderer;
        }


        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            SplineSettingsEditorUtility.RefreshSplines(RendererType);
#endif
        }
    }
}

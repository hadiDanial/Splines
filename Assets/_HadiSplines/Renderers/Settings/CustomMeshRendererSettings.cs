using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [CreateAssetMenu(fileName = "Custom Mesh Renderer Settings", menuName = "Splines/Renderers/Custom Mesh Settings")]
    public class CustomMeshRendererSettings : RendererSettings
    {
        [SerializeField]
        private Mesh mesh;

        public Mesh Mesh { get => mesh; }

        protected override void SetRendererType()
        {
            rendererType = SplineRendererType.CustomMeshRenderer;
        }
    }
}

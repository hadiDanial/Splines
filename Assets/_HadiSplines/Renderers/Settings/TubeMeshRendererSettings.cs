using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [CreateAssetMenu(fileName = "Tube MeshRenderer Settings", menuName = "Splines/Renderers/Tube Settings")]
    public class TubeMeshRendererSettings : RendererSettings
    {
        [SerializeField, Range(0.01f, 10f)]
        private float radius = 0.25f;

        public float Radius { get => radius; }

        protected override void SetRendererType()
        {
            this.rendererType = SplineRendererType.TubeMeshRenderer;
        }
    }
}

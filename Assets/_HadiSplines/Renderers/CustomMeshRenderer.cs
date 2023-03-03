using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public class CustomMeshRenderer : SplineMeshRenderer
    {
        CustomMeshRendererSettings settings;

        protected override string GetDefaultName()
        {
            return "Custom Mesh";
        }

        protected override int GetTriangleCount(int numPoints)
        {
            throw new System.NotImplementedException();
        }

        protected override void SetSettings(RendererSettings settings)
        {
            this.settings = (CustomMeshRendererSettings)settings;
            mesh = this.settings.Mesh;
            meshRenderer.sharedMaterial = settings.Material;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

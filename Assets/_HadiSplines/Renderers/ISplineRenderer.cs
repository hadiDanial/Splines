using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    internal interface ISplineRenderer
    {
        public void AddPoint(Vector3 point);
        public void SetPoint(int index, Vector3 point);
        public void SetData(SplineData splineData);        
        public void SetPointCount(int count);
        public void Clear();
        public void Setup();
        public void SetClosedShape(bool closed);
    }

    public enum SplineRendererType
    { 
        LineRenderer,
        MeshRenderer,
        QuadRenderer
    }

}
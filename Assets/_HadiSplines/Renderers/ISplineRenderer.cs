using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    internal interface ISplineRenderer
    {
        public void Setup();
        public void SetData(SplineData splineData);        
        public void SetPoint(int index, Vector3 point);
        public void SetPointCount(int count);
        public void SetClosedShape(bool closed);
        public void SetFill(bool isFilled = true);
        public void Clear();
    }

    public enum SplineRendererType
    { 
        LineRenderer,
        MeshRenderer,
        QuadRenderer
    }

}
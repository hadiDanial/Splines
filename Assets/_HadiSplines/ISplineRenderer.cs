using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    internal interface ISplineRenderer
    {
        public void AddPoint(Vector3 point);
        public void SetPoint(int index, Vector3 point);
        public void SetPoints(List<Vector3> points);
        public void SetPointCount(int count);
        public void Clear();
    }

    public enum SplineRendererType
    { 
        LineRenderer,
        MeshRenderer,
        QuadRenderer
    }

}
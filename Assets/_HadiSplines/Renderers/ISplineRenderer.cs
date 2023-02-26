using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    internal interface ISplineRenderer
    {
        /// <summary>
        /// Sets the spline renderer up.
        /// </summary>
        /// <param name="material">The material that the renderer should use.</param>
        public void Setup(Material material);

        /// <summary>
        /// Sets the spline data that the renderer will use to render the spline.
        /// </summary>
        /// <param name="splineData">Spline data (points, normals, tangents).</param>
        public void SetData(SplineData splineData); 

        /// <summary>
        /// Mark the spline as closed (loops).
        /// </summary>
        /// <param name="closed">True if the spline should be closed, false otherwise</param>
        public void SetClosedSpline(bool closed);

        /// <summary>
        /// Should the area inside the spline be filled?
        /// </summary>
        /// <param name="isFilled"></param>
        public void SetFill(bool isFilled = true);

        /// <summary>
        /// Clear the data inside the renderer.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Destroy the spline renderer and all of its related components.
        /// </summary>
        public void Destroy();

        public SplineRendererType GetRendererType();
    }

    public enum SplineRendererType
    { 
        LineRenderer,
        MeshRenderer,
        QuadRenderer,
        None
    }

}
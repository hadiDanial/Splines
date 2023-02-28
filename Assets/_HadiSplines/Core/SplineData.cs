using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [Serializable]
    public class SplineData
    {
        [SerializeField] private List<Vector3> points;
        [SerializeField] private List<Vector3> normals;
        [SerializeField] private List<Vector3> tangents;
        [SerializeField] private List<float> cumulativeLengthAtPoint;
        private float splineLength;
        public Transform objectTransform;
        public bool useWorldSpace;
        public List<Vector3> Points { get => points; private set => points = value; }
        public List<Vector3> Normals { get => normals; private set => normals = value; }
        public List<Vector3> Tangents { get => tangents; private set => tangents = value; }
        public float Length { get => splineLength; }
        public List<float> CumulativeLengthAtPoint { get => cumulativeLengthAtPoint; private set => cumulativeLengthAtPoint = value; }

        public SplineData()
        {
            points = new List<Vector3>();
            normals = new List<Vector3>();
            tangents = new List<Vector3>();
            cumulativeLengthAtPoint = new List<float>();
        }

        /// <summary>
        /// Calculates the length of the spline.
        /// </summary>
        public void CalculateLength()
        {
            splineLength = 0;
            cumulativeLengthAtPoint.Add(0);
            for (int i = 0; i < points.Count - 1; i++)
            {
                splineLength += Vector3.Distance(points[i], points[i + 1]);
                cumulativeLengthAtPoint.Add(splineLength);
            }
            //Debug.Log($"Spline length = {splineLength}");
        }

        /// <summary>
        /// Clears the spline data.
        /// </summary>
        internal void Clear()
        {
            points.Clear();
            tangents.Clear();
            normals.Clear();
            cumulativeLengthAtPoint.Clear();
            splineLength = 0;
        }
    }
}

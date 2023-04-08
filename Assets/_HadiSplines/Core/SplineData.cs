using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [Serializable]
    public class SplineData
    {
        [SerializeField] private List<Point> points;
        [SerializeField] private List<Vector3> segmentedPoints;
        [SerializeField] private List<Vector3> normals;
        [SerializeField] private List<Vector3> tangents;
        [SerializeField] private List<Vector3> scales;
        [SerializeField] private List<float> cumulativeLengthAtPoint;
        private float splineLength;
        public Transform objectTransform;
        public bool useObjectTransform;
        public RendererSettings settings;

        /// <summary>
        /// The list of points that define the spline.
        /// </summary>
        public List<Point> Points { get => points; set => points = value; }
        
        /// <summary>
        /// The segmented points made by subdividing the spline curves into segments.
        /// </summary>
        public List<Vector3> SegmentedPoints { get => segmentedPoints; private set => segmentedPoints = value; }
        
        /// <summary>
        /// The normals of the spline along the segmented points.
        /// </summary>
        public List<Vector3> Normals { get => normals; private set => normals = value; }
        
        /// <summary>
        /// The tangents of the spline along the segmented points.
        /// </summary>
        public List<Vector3> Tangents { get => tangents; private set => tangents = value; }
        
        /// <summary>
        /// The scale of the spline along the segmented points.
        /// </summary>
        public List<Vector3> Scale { get => scales; private set => scales = value; }

        /// <summary>
        /// The total length of the spline.
        /// </summary>
        public float Length { get => splineLength; }

        /// <summary>
        /// The total length along the spline up to each segmented point (starts at zero and goes up to `Length`.
        /// </summary>
        public List<float> CumulativeLengthAtPoint { get => cumulativeLengthAtPoint; private set => cumulativeLengthAtPoint = value; }

        public int numPoints;

        public SplineData()
        {
            points = new List<Point>();
            segmentedPoints = new List<Vector3>();
            normals = new List<Vector3>();
            tangents = new List<Vector3>();
            scales = new List<Vector3>();
            cumulativeLengthAtPoint = new List<float>();
        }

        /// <summary>
        /// Calculates the length of the spline.
        /// </summary>
        public void CalculateLength()
        {
            splineLength = 0;
            cumulativeLengthAtPoint.Add(0);
            for (int i = 0; i < segmentedPoints.Count - 1; i++)
            {
                splineLength += Vector3.Distance(segmentedPoints[i], segmentedPoints[i + 1]);
                cumulativeLengthAtPoint.Add(splineLength);
            }
        }

        /// <summary>
        /// Clears the spline data.
        /// </summary>
        internal void Clear()
        {
            segmentedPoints.Clear();
            tangents.Clear();
            normals.Clear();
            scales.Clear();
            cumulativeLengthAtPoint.Clear();
            splineLength = 0;
            numPoints = 0;
            settings = null;
        }
    }
}

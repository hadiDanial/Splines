using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [Serializable]
    public class SplineData
    {
        [SerializeField] private List<Vector3> segmentedPoints;
        [SerializeField] private List<Vector3> normals;
        [SerializeField] private List<Vector3> tangents;
        private float splineLength;

        public List<Vector3> SegmentedPoints { get => segmentedPoints; private set => segmentedPoints = value; }
        public List<Vector3> Normals { get => normals; private set => normals = value; }
        public List<Vector3> Tangents { get => tangents; private set => tangents = value; }

        public SplineData()
        {
            segmentedPoints = new List<Vector3>();
            normals = new List<Vector3>();
            tangents = new List<Vector3>();
        }

        public void CalculateLength()
        {
            splineLength = 0;
            for(int i = 0; i < segmentedPoints.Count - 1; i++)
            {
                splineLength += Vector3.Distance(segmentedPoints[i], segmentedPoints[i + 1]);
            }
            //Debug.Log($"Spline length = {splineLength}");
        }
        internal void Clear()
        {
            segmentedPoints.Clear();
            tangents.Clear();
            normals.Clear();
        }
    }
}

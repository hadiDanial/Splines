using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public struct SplineSegment
    {
        public float tBetweenPoints;
        public int pointIndex;

        public SplineSegment(float tBetweenPoints, int pointIndex)
        {
            this.tBetweenPoints = tBetweenPoints;
            this.pointIndex = pointIndex;
        }
    }
}

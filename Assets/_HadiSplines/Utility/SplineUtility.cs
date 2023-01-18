using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public static class SplineUtility
    {
        public static Vector3 LerpPosition(SplineData SplineData, SplineSegment segment)
        {
            if (segment.pointIndex >= SplineData.Points.Count - 1) 
                return SplineData.Points[SplineData.Points.Count - 1];
            return Vector3.Lerp(SplineData.Points[segment.pointIndex], SplineData.Points[segment.pointIndex + 1], segment.tBetweenPoints);
        }

        public static Vector3 LerpNormal(SplineData SplineData, SplineSegment segment)
        {
            if (segment.pointIndex >= SplineData.Normals.Count - 1)
                return SplineData.Normals[SplineData.Normals.Count - 1]; 
            return Vector3.Lerp(SplineData.Normals[segment.pointIndex], SplineData.Normals[segment.pointIndex + 1], segment.tBetweenPoints);
        }
        public static Vector3 LerpVelocity(SplineData SplineData, SplineSegment segment)
        {
            if (segment.pointIndex >= SplineData.Tangents.Count - 1)
                return SplineData.Tangents[SplineData.Tangents.Count - 1]; 
            return Vector3.Lerp(SplineData.Tangents[segment.pointIndex], SplineData.Tangents[segment.pointIndex + 1], segment.tBetweenPoints);
        }
    }
}

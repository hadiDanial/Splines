using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public static class SplineUtility
    {
        public static Vector3 LerpPosition(SplineData SplineData, SplineSegment segment, bool looped = false)
        {
            if (looped)
                return Vector3.Lerp(SplineData.SegmentedPoints[SplineData.SegmentedPoints.Count - 1], SplineData.SegmentedPoints[0], segment.tBetweenPoints);
            return Vector3.Lerp(SplineData.SegmentedPoints[segment.pointIndex], SplineData.SegmentedPoints[segment.pointIndex + 1], segment.tBetweenPoints);
        }

        public static Vector3 LerpNormal(SplineData SplineData, SplineSegment segment, bool looped = false)
        {
            if (looped)
                return Vector3.Lerp(SplineData.Normals[SplineData.Normals.Count - 1], SplineData.Normals[0], segment.tBetweenPoints);
            return Vector3.Lerp(SplineData.Normals[segment.pointIndex], SplineData.Normals[segment.pointIndex + 1], segment.tBetweenPoints);
        }
        public static Vector3 LerpVelocity(SplineData SplineData, SplineSegment segment, bool looped = false)
        {
            if (looped)
                return Vector3.Lerp(SplineData.Tangents[SplineData.Tangents.Count - 1], SplineData.Tangents[0], segment.tBetweenPoints);
            return Vector3.Lerp(SplineData.Tangents[segment.pointIndex], SplineData.Tangents[segment.pointIndex + 1], segment.tBetweenPoints);
        }

        public static SplineDataAtPoint GetDataAtSegment(SplineData SplineData, SplineSegment segment, bool looped)
        {
            Vector3 velocity = LerpVelocity(SplineData, segment, looped);
            return new SplineDataAtPoint(LerpPosition(SplineData, segment, looped), LerpNormal(SplineData, segment, looped), velocity.normalized, velocity, segment.pointIndex);
        }

        public static Vector3 TransformSplinePoint(this Transform transform, Vector3 point, bool useObjectTransform)
        {
            return useObjectTransform ? transform.TransformPoint(point) : point;
        }

        public static Vector3 InverseTransformSplinePoint(this Transform transform, Vector3 point, bool useObjectTransform)
        {
            return useObjectTransform ? transform.InverseTransformPoint(point) : point;
        }
        
        /// <summary>
        /// Calculates a position along a cubic bezier using DeCasteljau's method at percentage t (0-1).
        /// </summary>
        /// <param name="p0">Control Point of first anchor</param>
        /// <param name="p1">Anchor 1</param>
        /// <param name="p2">Control Point of second anchor</param>
        /// <param name="p3">Anchor 2</param>
        /// <param name="t">Percentage between 0 and 1</param>
        /// <returns></returns>
        public static Vector3 GetPosition_DeCasteljau(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 a, b, c, d, e;
            a = Vector3.Lerp(p0, p1, t);
            b = Vector3.Lerp(p1, p2, t);
            c = Vector3.Lerp(p2, p3, t);
            d = Vector3.Lerp(a, b, t);
            e = Vector3.Lerp(b, c, t);
            return Vector3.Lerp(d, e, t);
        }

    }
}

using System;
using UnityEngine;

namespace Hadi.Splines
{
    [Serializable]
    public class SplineSettings
    {        
        [SerializeField, Tooltip("Should the spline be closed?")]
        public bool closedSpline = false;
        [SerializeField, Tooltip("If true, the spline will be affected by the Transform component.")]
        public bool useObjectTransform = true;
        [SerializeField, Tooltip("This decides whether the spline will be constricted to a plane.")]
        public SplineMode splineMode = SplineMode.Full3D;
        [SerializeField, Tooltip("This decides whether the spline will be constricted to a plane.")]
        public SplineType splineType = SplineType.Cubic;
        [SerializeField, Tooltip("What to do if the spline receives a value outside of (0-1)?")]
        public EndOfSplineInstruction EndOfSplineInstruction = EndOfSplineInstruction.Loop;
        [SerializeField, Range(2, 25), Tooltip("How many segments there are between any two points. This decides the resolution of the spline.")]
        public int segmentsPerCurve = 10;
        [SerializeField, Tooltip("Distance between the last point and a newly added point.")]
        public float newPointDistance = 1.75f;
        [SerializeField, Tooltip("Should the point rotation be automatically calculated to match the tangent of the points?")]
        public bool automaticPointRotations = true;

        public object Clone()
        {
            SplineSettings settings = new SplineSettings();
            settings.closedSpline = closedSpline;
            settings.useObjectTransform = useObjectTransform;
            settings.splineMode = splineMode;
            settings.splineType = splineType;
            settings.EndOfSplineInstruction = EndOfSplineInstruction;
            settings.segmentsPerCurve = segmentsPerCurve;
            settings.newPointDistance = newPointDistance;
            return settings;
        }

        public override bool Equals(object obj)
        {
            return obj is SplineSettings settings &&
                   closedSpline == settings.closedSpline &&
                   useObjectTransform == settings.useObjectTransform &&
                   splineMode == settings.splineMode &&
                   splineType == settings.splineType &&
                   EndOfSplineInstruction == settings.EndOfSplineInstruction &&
                   segmentsPerCurve == settings.segmentsPerCurve &&
                   newPointDistance == settings.newPointDistance;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(closedSpline, useObjectTransform, splineMode, splineType, EndOfSplineInstruction, segmentsPerCurve, newPointDistance);
        }
    }

}

using UnityEngine;

namespace Hadi.Splines
{
    [System.Serializable]
    public class Point
    {
        public Vector3 anchor;
        public Vector3 controlPoint1, controlPoint2;
        public ControlMode mode;



        /// <summary>
        /// Constructor that initializes a point with mirrored control points.
        /// </summary>
        /// <param name="anchor">The anchor position of this point.</param>
        /// <param name="controlPoint1">The position of the first control point, which will be mirrored across the anchor.</param>
        /// <param name="controlPointPositionIsRelative">If true, the control point positions will be relative to the anchor point, otherwise they will be in world space.</param>
        public Point(Vector3 anchor, Vector3 controlPoint1, bool controlPointPositionIsRelative = true)
        {
            this.anchor = anchor;
            this.controlPoint2 = Vector3.zero;
            if (controlPointPositionIsRelative)
            {
                this.controlPoint1 = anchor + controlPoint1;
            }
            else
            {
                this.controlPoint1 = controlPoint1;
            }
            mode = ControlMode.Mirrored;
            MirrorPoints();

        }

        /// <summary>
        /// Constructor that initializes a point with broken control points.
        /// </summary>
        /// <param name="anchor">The anchor position of this point.</param>
        /// <param name="controlPoint1">The position of the first control point.</param>
        /// <param name="controlPoint2">The position of the second control point.</param>
        /// <param name="controlPointPositionIsRelative">If true, the control point positions will be relative to the anchor point, otherwise they will be in world space.</param>
        public Point(Vector3 anchor, Vector3 controlPoint1, Vector3 controlPoint2, bool controlPointPositionIsRelative = true)
        {
            this.anchor = anchor;
            if (controlPointPositionIsRelative)
            {
                this.controlPoint1 = anchor + controlPoint1;
                this.controlPoint2 = anchor + controlPoint2;
            }
            else
            {
                this.controlPoint1 = controlPoint1;
                this.controlPoint2 = controlPoint2;
            }
            this.mode = ControlMode.Broken;
        }

        /// <summary>
        /// Constructor that initializes a point with aligned control points.
        /// </summary>
        /// <param name="anchor">The anchor position of this point.</param>
        /// <param name="controlPoint1Direction">A direction vector for the first control point. This will be normalized.</param>
        /// <param name="controlPoint1Magnitude">Magnitude of first control point vector.</param>
        /// <param name="controlPoint2Magnitude">Magnitude of second control point vector.</param>
        public Point(Vector3 anchor, Vector3 controlPoint1Direction, float controlPoint1Magnitude, float controlPoint2Magnitude)
        {
            this.anchor = anchor;
            controlPoint1Direction = controlPoint1Direction.normalized;
            Vector3 vec1= controlPoint1Direction * controlPoint1Magnitude;
            Vector3 vec2= controlPoint1Direction * -controlPoint2Magnitude;

                this.controlPoint1 = anchor + vec1;
                this.controlPoint2 = anchor + vec2;
            
            this.mode = ControlMode.Aligned;
        }


        public void AlignPoints()
        {
            controlPoint2 = -controlPoint1.normalized * controlPoint2.magnitude;
        }

        public void MirrorPoints()
        {
            this.controlPoint2 = anchor - controlPoint1;
        }
    }
}

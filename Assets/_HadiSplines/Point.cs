using System;
using UnityEngine;

namespace Hadi.Splines
{
    [System.Serializable]
    public class Point
    {
        public Vector3 anchor;
        public Quaternion anchorRotation = Quaternion.identity;
        public Vector3 controlPoint1, controlPoint2;
        private Vector3 relativeControlPoint1, relativeControlPoint2;
        public ControlMode mode;

        /// <summary>
        /// Constructor that initializes a point with mirrored control points.
        /// </summary>
        /// <param name="anchor">The anchor position of this point.</param>
        /// <param name="controlPoint1">The position of the first control point, relative to the anchor, which will be mirrored across the anchor.</param>
        public Point(Vector3 anchor, Vector3 controlPoint1)
        {
            this.anchor = anchor;
            this.controlPoint2 = Vector3.zero;         
            relativeControlPoint1 = controlPoint1;            
            this.controlPoint1 = anchor + relativeControlPoint1;
            this.controlPoint2 = Vector3.zero;
            this.relativeControlPoint2 = Vector3.zero;
            mode = ControlMode.Mirrored;
            MirrorControlPoints();

        }

        /// <summary>
        /// Constructor that initializes a point with broken control points.
        /// </summary>
        /// <param name="anchor">The anchor position of this point.</param>
        /// <param name="controlPoint1">The position of the first control point, relative to the anchor.</param>
        /// <param name="controlPoint2">The position of the second control point, relative to the anchor.</param>
        public Point(Vector3 anchor, Vector3 controlPoint1, Vector3 controlPoint2)
        {
            this.anchor = anchor;
            relativeControlPoint1 = controlPoint1;
            relativeControlPoint2 = controlPoint2;
            this.controlPoint1 = anchor + relativeControlPoint1;
            this.controlPoint2 = anchor + relativeControlPoint2;
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

            relativeControlPoint1 = vec1;
            relativeControlPoint2 = vec2;
            this.controlPoint1 = anchor + relativeControlPoint1;
            this.controlPoint2 = anchor + relativeControlPoint2;
            this.mode = ControlMode.Aligned;
        }

        public void SetRotation(Quaternion rotation)
        {
            anchorRotation = rotation;
        }

        public void Refresh()
        {
            relativeControlPoint1 = controlPoint1 - anchor;
            relativeControlPoint2 = controlPoint2 - anchor;
            switch (mode)
            {
                case ControlMode.Aligned:
                    AlignControlPoints();
                    break;
                case ControlMode.Mirrored:
                    MirrorControlPoints();
                    break;
                default:
                    break;
            }
        }

        public void AlignControlPoints()
        {
            relativeControlPoint2 = -relativeControlPoint1.normalized * relativeControlPoint2.magnitude;
            controlPoint2 = relativeControlPoint2 + anchor;
        }

        public void MirrorControlPoints()
        {
            relativeControlPoint2 = -relativeControlPoint1;
            this.controlPoint2 = anchor - relativeControlPoint1;
        }

        public void Update(Vector3 anchor, Vector3 control1, Vector3 control2, Quaternion rotation)
        {
            bool refresh = false;
            if(!this.anchor.Equals(anchor))
            {
                this.anchor = anchor;
                this.controlPoint1 = anchor + relativeControlPoint1;
                this.controlPoint2 = anchor + relativeControlPoint2;
                refresh = true;
            }
            else if(!this.controlPoint1.Equals(control1) || !this.controlPoint2.Equals(control2) || !this.anchorRotation.Equals(rotation))
            {
                refresh = true;            
                this.controlPoint1 = control1;
                this.controlPoint2 = control2;
                this.anchorRotation = rotation;
            }

            if (refresh)
                Refresh();
        }
    }
}

using System;
using UnityEngine;

namespace Hadi.Splines
{
    [System.Serializable]
    public class Point
    {
        public Vector3 anchor;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 relativeControlPoint1, relativeControlPoint2;
        public ControlMode mode;

        /// <summary>
        /// Constructor that initializes a point with mirrored control points.
        /// </summary>
        /// <param name="anchor">The anchor position of this point.</param>
        /// <param name="controlPoint1">The position of the first control point, relative to the anchor, which will be mirrored across the anchor.</param>
        public Point(Vector3 anchor, Vector3 controlPoint1)
        {
            this.anchor = anchor;
            relativeControlPoint1 = controlPoint1;
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
            Vector3 vec1 = controlPoint1Direction * controlPoint1Magnitude;
            Vector3 vec2 = controlPoint1Direction * -controlPoint2Magnitude;

            relativeControlPoint1 = vec1;
            relativeControlPoint2 = vec2;
            this.mode = ControlMode.Aligned;
        }

        public void Refresh(SplineMode splineMode)
        {
            switch (splineMode)
            {
                case SplineMode.XY:
                    {
                        anchor = new Vector3(anchor.x, anchor.y);
                        relativeControlPoint1 = new Vector3(relativeControlPoint1.x, relativeControlPoint1.y);
                        relativeControlPoint2 = new Vector3(relativeControlPoint2.x, relativeControlPoint2.y);
                        break;
                    }
                case SplineMode.XZ:
                    {
                        anchor = new Vector3(anchor.x, 0, anchor.z);
                        relativeControlPoint1 = new Vector3(relativeControlPoint1.x, 0, relativeControlPoint1.z);
                        relativeControlPoint2 = new Vector3(relativeControlPoint2.x, 0, relativeControlPoint2.z);
                    }
                    break;
                case SplineMode.YZ:
                    {
                        anchor = new Vector3(0, anchor.y, anchor.z);
                        relativeControlPoint1 = new Vector3(0, relativeControlPoint1.y, relativeControlPoint1.z);
                        relativeControlPoint2 = new Vector3(0, relativeControlPoint2.y, relativeControlPoint2.z);
                    }
                    break;
                case SplineMode.Full3D:
                default:
                    break;
            }
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
        }

        public void MirrorControlPoints()
        {
            relativeControlPoint2 = -relativeControlPoint1;
        }

        public void UpdateAnchor(Vector3 anchor, Quaternion rotation)
        {
            this.anchor = anchor;
            this.rotation = rotation;
        }

        public void UpdateControlPoints(Vector3 control1, Vector3 control2)
        {
            relativeControlPoint1 = control1 - anchor;
            relativeControlPoint2 = control2 - anchor;
        }

        public void Update(SplineMode splineMode)
        {
            Refresh(splineMode);
        }

        public Vector3 GetControlPoint1() => anchor + relativeControlPoint1;
        public Vector3 GetControlPoint2() => anchor + relativeControlPoint2;
    }
}

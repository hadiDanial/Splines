using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public struct SplineDataAtPoint
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 tangent;
        public Vector3 velocity;

        public SplineDataAtPoint(Vector3 position, Vector3 normal, Vector3 tangent, Vector3 velocity)
        {
            this.position = position;
            this.normal = normal;
            this.tangent = tangent;
            this.velocity = velocity;
        }

        public override string ToString()
        {
            return $"Position: {position}, Normal: {normal}, Tangent: {tangent}";
        }
    }
}

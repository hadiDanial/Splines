using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [System.Serializable]
    public struct Curve
    {
        public Point P1, P2;

        public Curve(Point P1, Point P2)
        {
            this.P1 = P1;
            this.P2 = P2;
        }
    }
}

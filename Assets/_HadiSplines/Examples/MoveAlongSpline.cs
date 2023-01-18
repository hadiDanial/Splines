using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public class MoveAlongSpline : MonoBehaviour
    {
        public Spline spline;
        public float movementSpeed = 1;
        public float t = 0;
        public AnimationCurve easing;
        private void Update()
        {
            SplineDataAtPoint data = spline.GetDataAtPoint(easing.Evaluate(t % 1));
            transform.position = data.position;
            transform.rotation = Quaternion.LookRotation(data.tangent, data.normal);
            t += Time.deltaTime * movementSpeed;
        }
    }
}

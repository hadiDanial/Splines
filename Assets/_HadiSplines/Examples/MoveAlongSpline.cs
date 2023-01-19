using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public class MoveAlongSpline : MonoBehaviour
    {
        public Spline spline;
        public float movementSpeed = 2f;
        public float timePerLoop = 2f;
        public SplineMovementMode movementMode = SplineMovementMode.Time;
        public float t = 0;
        public AnimationCurve easing;
        private void Update()
        {
            SplineDataAtPoint data;
            if (movementMode == SplineMovementMode.Time)
            {
                data = spline.GetDataAtPoint(easing.Evaluate(t % 1));
                t += Time.deltaTime * (1 / timePerLoop);
            }
            else if (movementMode == SplineMovementMode.Distance)
            {
                data = spline.GetDataAtDistance(t);
                t += Time.deltaTime * movementSpeed;
            }
            else return;
            transform.position = data.position;
            transform.rotation = Quaternion.LookRotation(data.tangent, data.normal);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public class MoveAlongSpline : MonoBehaviour
    {
        [SerializeField]
        internal Spline spline;
        [SerializeField]
        internal SplineMovementMode movementMode = SplineMovementMode.Time;
        [SerializeField]
        internal float movementSpeed = 2f;
        [SerializeField]
        internal float timePerLoop = 2f;
        [SerializeField]
        internal AnimationCurve timeValueEasing;
        [SerializeField]
        internal float currentValue = 0;
        [SerializeField]
        internal int lastIndex = 0;

        private void Update()
        {
            SplineDataAtPoint data;
            if (movementMode == SplineMovementMode.Time)
            {
                data = MoveByTime();
            }
            else if (movementMode == SplineMovementMode.Distance)
            {
                data = MoveByDistance();
            }
            else return;
            Move(data);
        }

        protected virtual SplineDataAtPoint MoveByTime()
        {
            SplineDataAtPoint data = spline.GetDataAtPoint(timeValueEasing.Evaluate(currentValue % 1));
            currentValue += Time.deltaTime * (1 / timePerLoop);
            return data;
        }

        protected virtual SplineDataAtPoint MoveByDistance()
        {
            SplineDataAtPoint data = spline.GetDataAtDistance(currentValue, lastIndex);
            lastIndex = data.index;
            currentValue += Time.deltaTime * movementSpeed;
            return data;
        }

        protected virtual void Move(SplineDataAtPoint data)
        {
            transform.position = data.position;
            transform.rotation = Quaternion.LookRotation(data.tangent, data.normal);
        }
    }
}

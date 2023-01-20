using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public class SplineMover : MonoBehaviour
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
        internal AnimationCurve timeValueEasing = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField]
        internal float currentValue = 0;
        [SerializeField]
        internal int lastIndex = 0;

        private bool moveForwards = true;
        private bool isMoving = true;

        public bool MoveForwards { get => moveForwards; protected set => moveForwards = value; }
        public bool IsMoving { get => isMoving; protected set => isMoving = value; }

        protected virtual void Update()
        {
            SplineDataAtPoint data;
            if (!IsMoving) 
                return;
            if (movementMode == SplineMovementMode.Time)
            {
                data = MoveByTime();
            }
            else if (movementMode == SplineMovementMode.Distance)
            {
                data = MoveByDistance();
            }
            else return;
            IncrementTimer();
            Move(data);
        }

        protected virtual SplineDataAtPoint MoveByTime()
        {
            if (currentValue < 0) currentValue += 1;
            SplineDataAtPoint data = spline.GetDataAtPoint(timeValueEasing.Evaluate(currentValue % 1));
            return data;
        }

        protected virtual SplineDataAtPoint MoveByDistance()
        {
            if (currentValue < 0) currentValue += spline.Length;
            SplineDataAtPoint data = spline.GetDataAtDistance(currentValue, lastIndex, MoveForwards);
            lastIndex = data.index;
            return data;
        }

        protected virtual void IncrementTimer()
        {
            if (movementMode == SplineMovementMode.Time)
            {
                currentValue += Time.deltaTime * (1 / timePerLoop) * (MoveForwards ? 1 : -1);
            }
            else if (movementMode == SplineMovementMode.Distance)
            {
                currentValue += Time.deltaTime * movementSpeed * (MoveForwards ? 1 : -1);
            }
        }

        protected virtual void Move(SplineDataAtPoint data)
        {
            transform.position = data.position;
            transform.rotation = Quaternion.LookRotation(data.tangent, data.normal);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hadi.Splines
{
    public class FreeMovement : SplineMover
    {
        private void Awake()
        {
            IsMoving = false;
        }
        protected override SplineDataAtPoint MoveByDistance()
        {
            return base.MoveByDistance();
        }
        protected override void Move(SplineDataAtPoint data)
        {
            base.Move(data);
            transform.position = data.position;
        }

        public void OnMoveForwards(InputAction.CallbackContext context)
        {
            if (context.performed)
                IsMoving = true;
            if (context.canceled)
                IsMoving = false;
        }
        public void OnMoveBackwards(InputAction.CallbackContext context)
        {
            if (context.performed)
                MoveForwards = !MoveForwards;
        }
    }
}

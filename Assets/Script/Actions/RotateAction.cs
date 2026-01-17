using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class RotateAction : Action
    {
        private float rotateSpeed;
        private bool isRotateRight;
        private Transform subjectTransform;
        private int rightMultiplier = 1;
        
        public RotateAction(GameObject subject, bool blocking, float delay, float rotateSpeed, float duration, bool isRight) : base(subject,blocking, delay, duration)
        {
            this.rotateSpeed = rotateSpeed;
            isRotateRight = isRight;
            subjectTransform = subject.transform;
            if (isRotateRight)
            {
                rightMultiplier = -1;
            }
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            if (timePasses >= duration)
            {
                return true;
            }
            Rotating();
            return false;
        }

        private void Rotating()
        {
            subjectTransform.Rotate(0f, 0f,  rightMultiplier * rotateSpeed * Time.deltaTime);
        }
    }
}

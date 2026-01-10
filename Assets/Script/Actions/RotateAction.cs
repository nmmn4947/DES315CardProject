using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class RotateAction : Action
    {
        private float rotateSpeed;
        private float rotateDuration;
        private bool isRotateRight;
        public void SetRotateSpeed(float rotatespd) { rotateSpeed = rotatespd; }
        public void SetRotateDuration(float rotatedur) { rotateDuration = rotatedur; }
        public void SetRotateRight(bool rt) { isRotateRight = rt; }

        Transform subjectTransform;
        int rightMultiplier = 1;
        bool infinite = false;
        
        public RotateAction(bool blocking, bool endWPrevious, float delay, float rotateSpeed, float rotateDuration, bool isRight) : base(blocking, endWPrevious, delay)
        {
            this.rotateSpeed = rotateSpeed;
            this.rotateDuration = rotateDuration;
            isRotateRight = isRight;
        }
        
        public override void SetUp()
        {
            subjectTransform = subject.GetComponent<Transform>();
            if (isRotateRight)
            {
                rightMultiplier = -1;
            }

            if (rotateDuration < 0)
            {
                infinite = true;
            }
        }

        public override bool UpdateLogicUntilDone()
        {
            if (infinite)
            {
                if (endWithPrevious)
                {
                    
                }
            }
            else
            {
                rotateDuration -= Time.deltaTime;
                if (rotateDuration < 0)
                {
                    return true;
                }
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

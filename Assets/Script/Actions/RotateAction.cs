using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CardProject
{
    public class RotateAction : Action
    {
        private float angleCalculation;
        private int rightMultiplier = 1;
        private Transform subjectTransform;
        private float startingAngle;
        private float finalAngle;
        private bool once = false;
        
        public RotateAction(GameObject subject, bool blocking, float delay, float duration, float goalAngle, int loopCountMultiplier, bool isRight) : base(subject, blocking, delay, duration)
        {
            subjectTransform = subject.transform;
            if (isRight)
            {
                rightMultiplier = -1;
            }
            angleCalculation = goalAngle + (loopCountMultiplier * 360.0f);
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            subjectTransform.localRotation = Quaternion.Euler(subjectTransform.eulerAngles.x, subjectTransform.eulerAngles.y, AngleEaseOutQuad());
            return timePasses > duration;
        }

        private float AngleLerping()
        {
            float currentAngle = Mathf.SmoothStep(startingAngle, finalAngle, timePasses/duration);
            return currentAngle;
        }

        private float AngleEaseOutQuad()
        {
            float currentAngle = Mathf.Lerp(startingAngle, finalAngle, EaseOutExpo());
            return currentAngle;
        }

        protected override void RunOnceBeforeUpdate()
        {
            startingAngle = subjectTransform.localEulerAngles.z;
            finalAngle = angleCalculation * rightMultiplier;
        }
        
        
    }
}

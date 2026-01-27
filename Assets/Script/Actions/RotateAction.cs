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
        private bool optimized = false;
        private Quaternion startRotation;
        private float totalAngleDelta;
        private Quaternion targetRotation;
        
        public RotateAction(GameObject subject, bool blocking, float delay, float duration, float goalAngle, int loopCountMultiplier, bool isRight) : base(subject, blocking, delay, duration)
        {
            subjectTransform = subject.transform;
            if (isRight)
            {
                rightMultiplier = -1;
            }
            angleCalculation = goalAngle + (loopCountMultiplier * 360.0f);
        }
        
        public RotateAction(GameObject subject, bool blocking, float delay, float duration, float goalAngle) : base(subject, blocking, delay, duration)
        {
            subjectTransform = subject.transform;
            angleCalculation = goalAngle;
            optimized = true;
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            //Try to use Quarternion.Lerp
            if (optimized)
            {
                float maxDegreesDelta = startingAngle + (totalAngleDelta * EaseOutExpo());
                //subjectTransform.localRotation = Quaternion.RotateTowards(subjectTransform.localRotation, targetRotation, maxDegreesDelta);
                //float currentAngle = Mathf.Lerp(startingAngle, startingAngle + totalAngleDelta, EaseOutExpo());
                float currentAngle = startingAngle + (totalAngleDelta * EaseOutExpo());
                subjectTransform.localRotation = Quaternion.Euler(subjectTransform.localEulerAngles.x, subjectTransform.localEulerAngles.y, currentAngle);
                return timePasses > duration;
            }
            else
            {
                subjectTransform.localRotation = Quaternion.Euler(subjectTransform.localEulerAngles.x, subjectTransform.localEulerAngles.y, AngleEaseOutQuad());
                return timePasses > duration;
            }
            
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
            startRotation = subjectTransform.localRotation;
            
            /*targetRotation = startRotation * Quaternion.Euler(0, 0, finalAngle);
            totalAngleDelta = Quaternion.Angle(startRotation, targetRotation);*/

            totalAngleDelta = angleCalculation - startingAngle;
            while (totalAngleDelta > 180f) totalAngleDelta -= 360f;
            while (totalAngleDelta < -180f) totalAngleDelta += 360f;
        }
        
        
    }
}

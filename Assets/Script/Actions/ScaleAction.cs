using System;
using UnityEngine;

namespace CardProject
{
    public class ScaleAction : Action
    {
        private Vector2 finalScale;
        private Transform subjectTransform;
        private Vector2 originalScale;
        
        public ScaleAction(GameObject subject, bool blocking, float delay, Vector2 finalScale, float duration, Func<float ,float> easingFunc) : base(subject, blocking, delay, duration, easingFunc)
        {
            this.finalScale = finalScale;
            this.duration = duration;
            subjectTransform = subject.transform;
            originalScale = subjectTransform.localScale;
            actionName = "Scale";
        }

        protected override void RunOnceBeforeUpdate()
        {
            originalScale = subjectTransform.localScale;
        }
        
        protected override bool UpdateLogicUntilDone(float dt)
        {
            return ScaleUntilFinalScale();
        }

        

        //EaseOutExpo
        private bool ScaleUntilFinalScale()
        {
            subjectTransform.localScale = new Vector3(Mathf.Lerp(originalScale.x, finalScale.x, easingTime),
                                                      Mathf.Lerp(originalScale.y, finalScale.y, easingTime), 0);
            
            // Snap to final scale when very close or time is up
            if (percentageDone >= 1.0f || easingTime >= 0.999f)
            {
                subjectTransform.localScale = new Vector3(finalScale.x, finalScale.y, 0);
                return true;
            }

            return false;
        }
    }
}

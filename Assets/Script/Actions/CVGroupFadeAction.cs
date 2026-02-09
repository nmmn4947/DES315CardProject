using System;
using UnityEngine;

namespace CardProject
{
    public class CVGroupFadeAction : Action
    {
        private float startingAlpha;
        private float targetAlpha;
        private CanvasGroup canvasGroup;
        private bool cvNull = false;
        public CVGroupFadeAction(GameObject subject,bool blocking, float delay, float duration, Func<float, float> easingFunction,float targetAlpha) : base(subject, blocking, delay, duration, easingFunction)
        {
            this.targetAlpha = targetAlpha;
            canvasGroup = subject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogError("canvasGroup is null on FadeAction");
                cvNull = true;
            }
            else
            {
                startingAlpha = canvasGroup.alpha;
            }
            actionName = "CVGroupFade";
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            if (cvNull) return true;
            
            canvasGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, easingTime);
            
            return (timePasses > duration);
        }
    }
}

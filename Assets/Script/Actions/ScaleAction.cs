using UnityEngine;

namespace CardProject
{
    public class ScaleAction : Action
    {
        private Vector2 finalScale;
        private Transform subjectTransform;
        private Vector2 originalScale;
        
        public ScaleAction(GameObject subject, bool blocking, float delay, Vector2 finalScale, float duration) : base(subject, blocking, delay, duration)
        {
            this.finalScale = finalScale;
            this.duration = duration;
            subjectTransform = subject.transform;
            originalScale = subjectTransform.localScale;
            actionName = "Scale";
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            return ScaleUntilFinalScale();
        }

        private bool ScaleUntilFinalScale()
        {
            subjectTransform.localScale = new Vector3(Mathf.Lerp(originalScale.x, finalScale.x, EaseOutExpo()),
                                                      Mathf.Lerp(originalScale.y, finalScale.y, EaseOutExpo()), 0);
            
            // Snap to final scale when very close or time is up
            if (percentageDone >= 1.0f || EaseOutExpo() >= 0.999f)
            {
                subjectTransform.localScale = new Vector3(finalScale.x, finalScale.y, 0);
                return true;
            }

            return false;
        }
    }
}

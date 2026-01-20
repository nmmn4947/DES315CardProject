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
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            return ScaleUntilFinalScale();
        }

        private bool ScaleUntilFinalScale()
        {
            subjectTransform.localScale = new Vector3(Mathf.Lerp(originalScale.x, finalScale.x, timePasses/duration),
                                                      Mathf.Lerp(originalScale.y, finalScale.y, timePasses/duration), 0);
            
            Vector2 newScale = subjectTransform.localScale;
            if (Vector2.Distance(newScale, finalScale) > 0.001f)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

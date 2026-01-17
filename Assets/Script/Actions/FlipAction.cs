using UnityEngine;

namespace CardProject
{
    public class FlipAction : Action
    {
        private Transform subjectTransform;
        private float originalYrotation;
        private Quaternion startRotation;
        private Quaternion endRotation;
        
        public FlipAction(GameObject subject, bool blocking, float delay, float duration) :  base(subject, blocking, delay, duration)
        {
            subjectTransform = subject.transform;
            startRotation = subjectTransform.localRotation;
            endRotation = startRotation * Quaternion.Euler(0f, 180f, 0f);
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            return FlippingUntilDone();
        }

        private bool FlippingUntilDone()
        {
            float clamp = Mathf.Clamp01(timePasses/duration);
            subjectTransform.localRotation = Quaternion.Lerp(startRotation, endRotation, clamp);
            return (timePasses/duration) >= 1f;
        }
    }
}

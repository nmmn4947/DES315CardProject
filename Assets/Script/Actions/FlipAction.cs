using UnityEngine;

namespace CardProject
{
    public class FlipAction : Action
    {
        private Transform subjectTransform;
        private float originalYrotation;
        private Quaternion startRotation;
        private Quaternion endRotation;
        private float startY;
        private float endY;
        
        public FlipAction(GameObject subject, bool blocking, float delay, float duration) :  base(subject, blocking, delay, duration)
        {
            subjectTransform = subject.transform;
            actionName = "Flip";
        }

        protected override void RunOnceBeforeUpdate()
        {
            startY = subjectTransform.localEulerAngles.y;
            endY = startY + 180f;
            startRotation = subjectTransform.localRotation;
            endRotation = startRotation * Quaternion.Euler(0f, 180f, 0f);
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            return FlippingUntilDone();
            
            float t = Mathf.Clamp01(timePasses / duration);
            
            // Lerp only the Y rotation, preserve X and Z
            float currentY = Mathf.Lerp(startY, endY, t);
            
            // Only update Y, keep current X and Z
            Vector3 currentEuler = subjectTransform.localEulerAngles;
            subjectTransform.localRotation = Quaternion.Euler(currentEuler.x, currentY, currentEuler.z);
            //subjectTransform.localRotation = subjectTransform.localRotation * Quaternion.Euler(0f, 180f * dt / duration, 0f);
            
            return (timePasses / duration) >= 1f;
            
        }
        
        private bool FlippingUntilDone()
        {
            float clamp = Mathf.Clamp01(timePasses/duration);
            subjectTransform.localRotation = Quaternion.Lerp(startRotation, endRotation, clamp); // linear
            return (timePasses/duration) >= 1f;
        }
    }
}

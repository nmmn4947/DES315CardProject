using UnityEngine;
using System;

namespace CardProject
{
    public class MoveAction : Action
    {
        private Vector3 destination;
        private Transform subjectTransform;
        private Vector2 originalPosition;
        private float speed;

        public MoveAction(GameObject subject, bool blocking, float delay, float speed, Vector3 destination) : base(subject,blocking, delay, Vector2.Distance(subject.transform.position, destination)/speed)
        {
            this.destination = destination;
            subjectTransform = this.subject.transform;
            originalPosition = this.subject.transform.position;
            this.speed = speed;
        }
        
        public MoveAction(float duration, GameObject subject, bool blocking, float delay, Vector3 destination) : base(subject,blocking, delay, duration)
        {
            this.destination = destination;
            subjectTransform = this.subject.transform;
            if (duration > 0f)
            {
                this.speed = Vector2.Distance(subject.transform.position, destination) / duration;
            }
            else
            {
                this.speed = 0f;
            }
        }

        protected override void RunOnceBeforeUpdate()
        {
            originalPosition = this.subject.transform.position;
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            if (duration <= 0f)
            {
                subjectTransform.position = destination;
                return true;
            }
            subjectTransform.position = Vector3.LerpUnclamped(originalPosition, destination, EaseOutBackTime());

            return percentageDone >= 1f;
            
        }

        private float PositionEaseOutBack(float clampedTime)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            float ret = 1 + c3 * Mathf.Pow(clampedTime - 1, 3) + c1 * Mathf.Pow(clampedTime - 1, 2); ;
            
            return ret;
        }
    }
}

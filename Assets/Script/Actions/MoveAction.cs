using UnityEngine;
using System;

namespace CardProject
{
    public class MoveAction : Action
    {
        private Vector3 destination;
        private Transform subjectTransform;
        private Vector2 originalPosition;

        public MoveAction(GameObject subject, bool blocking, float delay, float duration, Vector2 destination) : base(subject,blocking, delay, duration)
        {
            this.destination = destination;
            subjectTransform = this.subject.transform;
            originalPosition = this.subject.transform.position;
        }
        
        protected override bool UpdateLogicUntilDone(float dt)
        {
            if (Vector3.Distance(subjectTransform.position, destination) > 0.001f)
            {
                MoveTowardsDestination(dt);
                return false;
            }
            else
            {
                return true;
            }
            
        }

        private void MoveTowardsDestination(float dt)
        {
            subjectTransform.position = Vector2.Lerp(subjectTransform.position, destination, speed * dt);
        }
    }
}

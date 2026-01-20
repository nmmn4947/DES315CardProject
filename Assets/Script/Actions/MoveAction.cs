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

        public MoveAction(GameObject subject, bool blocking, float delay, float speed, Vector2 destination) : base(subject,blocking, delay, Vector2.Distance(subject.transform.position, destination)/speed)
        {
            this.destination = destination;
            subjectTransform = this.subject.transform;
            originalPosition = this.subject.transform.position;
            this.speed = speed;
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
            subjectTransform.position = Vector2.MoveTowards(subjectTransform.position, destination, speed * dt);
        }
    }
}

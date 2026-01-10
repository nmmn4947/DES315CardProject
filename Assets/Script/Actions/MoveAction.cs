using UnityEngine;
using System;

namespace CardProject
{
    public class MoveAction : Action
    {
        private float speed;
        private Vector3 destination;
        public void SetSpeed(float spd) { speed = spd; }
        public void SetDestination(Vector2 dest) { destination = dest; }
        
        Transform subjectTransform;

        public MoveAction(bool blocking, float delay, float speed, Vector2 destination) : base(blocking, delay)
        {
            this.speed = speed;
            this.destination = destination;
        }
        
        public override void SetUp()
        {
            subjectTransform = subject.gameObject.GetComponent<Transform>();
        }
        
        public override bool UpdateLogicUntilDone()
        {
            if (Vector3.Distance(subjectTransform.position, destination) > 0.001f)
            {
                MoveTowardsDestination();
                return false;
            }
            else
            {
                return true;
            }
            
        }

        private void MoveTowardsDestination()
        {
            subjectTransform.position = Vector3.MoveTowards(subjectTransform.position, destination, speed * Time.deltaTime);
        }
    }
}

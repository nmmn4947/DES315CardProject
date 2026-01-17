using UnityEngine;
using System;
using System.Collections.Generic;

namespace CardProject
{
    public abstract class Action
    {
        // GENERAL ACTION VARIABLE
        public bool blocking;
        public float delay;
        public float timePasses = 0.0f;
        public float duration;
        private int percentageDone;
        
        protected GameObject subject;
        
        protected Action(GameObject subject, bool blocking, float delay, float duration)
        {
            this.subject = subject;
            this.blocking = blocking;
            this.delay = delay;
            this.duration = duration;
        }

        static public void SynchronizeDurationFirstToSecond(Action action1, Action action2)
        {
            action2.duration = action1.duration;
        }
        
        protected abstract bool UpdateLogicUntilDone(float dt);
        public bool UpdateUntilDone(float dt)
        {
            if (RunDelayUntilDone(dt))
            {
                if (!UpdateLogicUntilDone(dt))
                {
                    timePasses += dt;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        private bool RunDelayUntilDone(float dt)
        {
            delay -= Time.deltaTime;
            if (delay <= 0.0f)
            {
                return true;
            }
            return false;
        }
    }
}

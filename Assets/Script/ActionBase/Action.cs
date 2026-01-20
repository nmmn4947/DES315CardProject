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
        public float percentageDone;  // 0 - 1
        
        protected GameObject subject;

        private int GroupFlag = -1;
        
        protected Action(bool blocking, float delay, float duration)
        {
            this.subject = null;
            this.blocking = blocking;
            this.delay = delay;
            this.duration = duration;
        }
        
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
                    timePasses += dt; //update timePasses
                    
                    percentageDone = timePasses / duration; //Updating percentageDone 0 - 1.
                    if (timePasses > duration)
                    {
                        percentageDone = 1.0f;
                    }
                    
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
        protected float GetTimeLeft()
        {
            if (timePasses > duration)
            {
                return 0f;
            }
            else
            {
                return duration - timePasses;
            }
        }
    }
}

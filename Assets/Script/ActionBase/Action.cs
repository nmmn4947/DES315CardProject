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

        private bool runEnterOnce = false;
        private float clampedTime;
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

        public void SynchronizeDurationFromThisAction(Action action)
        {
            duration = action.duration;
        }
        
        protected abstract bool UpdateLogicUntilDone(float dt);
        
        public bool UpdateUntilDone(float dt)
        {
            if (RunDelayUntilDone(dt))
            {
                Enter();
                if (!UpdateLogicUntilDone(dt))
                {
                    timePasses += dt; //update timePasses
                    clampedTime = Mathf.Clamp01(timePasses / duration);
                    
                    percentageDone = timePasses / duration; //Updating percentageDone 0 - 1.
                    if (timePasses > duration)
                    {
                        percentageDone = 1.0f;
                    }
                    
                    return false;
                }
                else
                {
                    RunOnceAfterUpdate();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        protected virtual void RunOnceBeforeUpdate() { }
        protected virtual void RunOnceAfterUpdate() { }

        private void Enter()
        {
            if (!runEnterOnce)
            {
                RunOnceBeforeUpdate();
                runEnterOnce = true;
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
            if (duration >= float.MaxValue)
            {
                return float.MaxValue;
            }
            
            if (timePasses > duration)
            {
                return 0f;
            }
            else
            {
                return duration - timePasses;
            }
        }

        #region EasingCode

        protected float EaseOutQuad()
        {
            float t = 1f - ((1f - clampedTime) * (1f - clampedTime));
            return t;
        }
        
        protected float EaseOutBack()
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            
            float ret = 1 + c3 * Mathf.Pow(clampedTime - 1, 3) + c1 * Mathf.Pow(clampedTime - 1, 2);
            return ret;
        }

        protected float EaseOutExpo()
        {
            return clampedTime >= 0.99f ? 1f : 1f - Mathf.Pow(2f, -10f * clampedTime);
        }

        #endregion

    }
}

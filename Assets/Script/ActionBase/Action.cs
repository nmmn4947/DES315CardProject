using UnityEngine;
using System;
using System.Collections.Generic;

namespace CardProject
{
    public abstract class Action
    {
        // GENERAL ACTION VARIABLE
        public bool blocking;
        public bool endWithPrevious;
        public float delay;
        
        protected GameObject subject;

        public Action(bool bkin, bool ewp, float dela)
        {
            blocking = bkin;
            endWithPrevious = ewp;
            delay = dela;
        }
        
        public Action(bool bkin, float dela)
        {
            blocking = bkin;
            delay = dela;
        }
        
        public void SetSubJect(GameObject subj)
        {
            subject = subj;
        }

        public abstract void SetUp();
        public abstract bool UpdateLogicUntilDone();
        public bool UpdateUntilDone()
        {
            if (RunDelayUntilDone())
            {
                if (!UpdateLogicUntilDone())
                {
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
        private bool RunDelayUntilDone()
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

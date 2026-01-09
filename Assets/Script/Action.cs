using UnityEngine;
using System;

namespace CardProject
{
    [Serializable]
    public abstract class Action
    {
        public bool startWithPrevious;
        public float delayBeforeACTION;
        
        private GameObject subject;
        public abstract void SetSubJect(GameObject subj);
        public abstract bool UpdateLogicUntilDone();
        public bool UpdateUntilDone()
        {
            if (RUNDelay())
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
        private bool RUNDelay()
        {
            delayBeforeACTION -= Time.deltaTime;
            if (delayBeforeACTION <= 0.0f)
            {
                return true;
            }
            return false;
        }
    }
}

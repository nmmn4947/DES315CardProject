using UnityEngine;

namespace CardProject
{
    public class WaitAction : Action
    {
        public WaitAction(float duration) : base(true, 0.0f, duration)
        {
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            if (timePasses > duration)
            {
                return true;
            }
            return false;
        }
    }
}

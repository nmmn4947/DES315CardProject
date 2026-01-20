using UnityEngine;
using System.Collections.Generic;

namespace CardProject
{
    public class NestedAction : Action
    {
        private ActionList nestedList = new ActionList();

        public NestedAction(bool blocking, float delay) : base(blocking, delay, float.MaxValue) { }
        public NestedAction(Action[] actions, bool blocking, float delay) : base(blocking, delay, float.MaxValue)
        {
            foreach (Action action in actions)
            {
                nestedList.AddAction(action);
            }
        }
        public NestedAction(List<Action> actions, bool blocking, float delay) : base(blocking, delay, float.MaxValue)
        {
            foreach (Action action in actions)
            {
                nestedList.AddAction(action);
            }
        }

        public void AddAction(Action action)
        {
            nestedList.AddAction(action);
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            nestedList.RunActions(dt);
            if (nestedList.IsEmpty())
            {
                return true;
            }
            return false;
        }
    }
}

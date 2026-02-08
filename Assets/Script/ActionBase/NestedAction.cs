using UnityEngine;
using System.Collections.Generic;
using Napadol.Tools;

namespace CardProject
{
    public class NestedAction : Action
    {
        private ActionList nestedList = new ActionList();

        public NestedAction(bool blocking, float delay) : base(blocking, delay, float.MaxValue) { actionName = "Nested"; }
        public NestedAction(Action[] actions, bool blocking, float delay) : base(blocking, delay, float.MaxValue)
        {
            foreach (Action action in actions)
            {
                nestedList.AddAction(action);
                
            }
            actionName = "Nested";
            easingFunction = Easing.EaseLinear;
        }
        public NestedAction(List<Action> actions, bool blocking, float delay) : base(blocking, delay, float.MaxValue)
        {
            foreach (Action action in actions)
            {
                nestedList.AddAction(action);
            }
            actionName = "Nested";
            easingFunction = Easing.EaseLinear;
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

        public ActionList GetActionList()
        {
            return nestedList;
        }

        public override string GetDebugText()
        {
            string s = "";
            s += actionName;
            s += "\n";
            for (int i = 0; i < nestedList.GetActionListCount(); i++)
            {
                s += " ";
                s += nestedList.GetTheList()[i].GetDebugText();
            }
            return s;
        }
    }
}

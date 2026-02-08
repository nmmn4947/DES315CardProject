using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class CallBackAction : Action
    {
        private System.Action actionToCallBack;
        private string nameOfFunc;
        private ActionList actionList;
        private int index;
        
        public CallBackAction(System.Action actionToCallBack, string nameOfFunc, bool blocking, float delay) : base(blocking, delay, 0.0f)
        {
            this.actionToCallBack = actionToCallBack;
            this.nameOfFunc = nameOfFunc;
            actionName = "CallBack";
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            actionToCallBack();
            return true;
        }

        public override string GetDebugText()
        {
            string s = "";
            s += "CallBack(";
            s += nameOfFunc;
            s += ")";
            s += "\n";
            return s;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class CallBackAction : Action
    {
        private System.Action actionToCallBack;
        private ActionList actionList;
        private int index;
        
        public CallBackAction(System.Action actionToCallBack, bool blocking, float delay, float duration) : base(blocking, delay, duration)
        {
            this.actionToCallBack = actionToCallBack;
            actionName = "CallBack";
        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            actionToCallBack();
            return true;
        }
    }
}

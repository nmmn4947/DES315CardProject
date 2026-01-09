using UnityEngine;
using System;

namespace CardProject
{
    [Serializable]
    public class TestAction : Action
    {
        public override void SetSubJect(GameObject subj)
        {
            
        }

        public override bool UpdateLogicUntilDone()
        {
            Debug.Log("Test");
            return false;
        }
        
        
    }
}

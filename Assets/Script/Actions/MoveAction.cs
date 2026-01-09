using UnityEngine;
using System;

namespace CardProject
{
    [Serializable]
    public class MoveAction : Action
    {
        [SerializeField] private float speed;
        public override void SetSubJect(GameObject subj)
        {
            
        }

        public override bool UpdateLogicUntilDone()
        {
            Debug.Log("MoveAction");
            return false;
        }
        
        
    }
}

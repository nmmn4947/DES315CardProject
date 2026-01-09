using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class ActionList : MonoBehaviour
    {
        [ActionAttribute(typeof(Action)), SerializeField] public ActionContainer _actions;
        //private Queue<ACTION> queueActions = new Queue<ACTION>();
        private List<Action> performingAction = new List<Action>();
        

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            PrepareAllActions();
            ResetPerformingAction();
        }

        // Update is called once per frame
        void Update()
        {
            if (RunPerformingAction())
            {
                ResetPerformingAction();
            }
        }

        private void PrepareAllActions()
        {
            foreach (Action action in _actions.actions)
            {
                action.SetSubJect(this.gameObject);
            }
        }
    
        private void ResetPerformingAction()
        {
            performingAction.Clear();
            //ADD THE FIRST
            if (_actions.actions.Count > 0)
            {
                performingAction.Add(_actions.actions[0]);
            }
        
            while (_actions.actions.Count > 0)
            {
                if (_actions.actions[0].startWithPrevious)
                {
                    performingAction.Add(_actions.actions[0]);
                    _actions.actions.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            return;
        }

        private bool RunPerformingAction()
        {
            bool isAllDone = true;
            foreach (Action action in performingAction)
            {
                if (!action.UpdateUntilDone())
                {
                    isAllDone = false;
                    break;
                }
            }
            return isAllDone;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class ActionList : MonoBehaviour
    {
        [SerializeReference] public List<Action> actions = new List<Action>();
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
            foreach (Action action in actions)
            {
                action.SetSubJect(this.gameObject);
            }
        }
    
        private void ResetPerformingAction()
        {
            performingAction.Clear();
            //ADD THE FIRST
            if (actions.Count > 0)
            {
                performingAction.Add(actions[0]);
            }
        
            while (actions.Count > 0)
            {
                if (actions[0].startWithPrevious)
                {
                    performingAction.Add(actions[0]);
                    actions.RemoveAt(0);
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

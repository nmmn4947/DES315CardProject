using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class ActionList
    {
        /*public ActionContainer _actions;
        private List<Action> performingAction = new List<Action>();*/
        private List<Action> actions = new List<Action>();

        public void RunActions()
        {
            List<int> indexesToKill = new List<int>();
            for(int i = 0; i < actions.Count; i++)
            {
                if (actions[i].UpdateUntilDone())
                {
                    indexesToKill.Add(i);
                }
                if (actions[i].blocking)
                {
                    break;
                }
            }

            for (int i = indexesToKill.Count - 1; i >= 0; i--)
            {
                actions.RemoveAt(indexesToKill[i]);
            }
        }
        
        public void InitActionList(GameObject gameObject)
        {
            foreach (Action action in actions)
            {
                action.SetSubJect(gameObject);
                action.SetUp();
            }
        }

        public void AddAction(Action action)
        {
            actions.Add(action);
        }

    }
}

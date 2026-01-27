using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class ActionList
    {
        private List<Action> actions = new List<Action>();

        public void RunActions(float dt)
        {
            List<int> indexesToKill = new List<int>();
            for(int i = 0; i < actions.Count; i++)
            {
                if (actions[i].UpdateUntilDone(dt))
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
        public void AddAction(Action action)
        {
            actions.Add(action);
        }
        public bool IsEmpty()
        {
            if (actions.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public int GetActionListCount()
        {
            return actions.Count;
        }

        public List<Action> GetTheList()
        {
            return actions;
        }

    }
}

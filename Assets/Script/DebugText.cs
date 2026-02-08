using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CardProject
{
    public class DebugText : MonoBehaviour
    {
        [SerializeField] private ActionListManager actionListObject;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private int lineOffset;
        [SerializeField] private RectTransform canvasRect;
        private ActionList actionList;
        private int amountOfActionsOnScreen;
        private string leftOver;
        private int currentLine;

        private void Start()
        {
            amountOfActionsOnScreen = (int)(canvasRect.sizeDelta.y/lineOffset) - 1;
            actionList = actionListObject.actionList;
        }

        void Update()
        {
            if (!actionList.IsEmpty())
            {
                string s = "";
                int i = 0;
                while (i <= amountOfActionsOnScreen)
                {
                    if (actionList.GetActionListCount() <= 0 || i >= actionList.GetActionListCount())
                    {
                        break;
                    }
                    Action currAction = actionList.GetTheList()[i];
                    s += currAction.GetDebugText();
                    i++;
                    if (currAction is NestedAction)
                    {
                        NestedAction n = currAction as NestedAction;
                        i += n.GetActionList().GetActionListCount() - 1;
                    }
                }
                
                var lines = s.Split('\n');
                string finalString = string.Join("\n", lines.Take(amountOfActionsOnScreen));
                if (lines.Length - amountOfActionsOnScreen > 0)
                {
                    finalString += "\nThere is ";
                    finalString += lines.Length - amountOfActionsOnScreen;
                    finalString += " Action(s) left.";
                }
                text.text = finalString;
            }
            else
            {
                text.text = "";
            }
        }
    }
}

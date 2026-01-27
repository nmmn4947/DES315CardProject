using TMPro;
using UnityEngine;

namespace CardProject
{
    public class DebugText : MonoBehaviour
    {
        [SerializeField] private CardManager cm;
        private TextMeshProUGUI text;
        private ActionList actionList;
        void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            actionList = cm.GetActionList();
            if (!actionList.IsEmpty())
            {
                string s = "";
                for (int i = 0; i < actionList.GetActionListCount(); i++)
                {
                    Action currAction = actionList.GetTheList()[i];
                    s += actionList.GetTheList()[i].actionName;
                    s += " ";
                    s += actionList.GetTheList()[i].percentageDone.ToString("F2");
                    s += "\n";
                    if (currAction.actionName == "Nested")
                    {
                        NestedAction nestedAction = currAction as NestedAction;
                        for (int y = 0; y < nestedAction.GetActionList().GetActionListCount(); y++)
                        {
                            s += "  ";
                            s += nestedAction.GetActionList().GetTheList()[i].actionName;
                            s += " ";
                            s += nestedAction.GetActionList().GetTheList()[i].percentageDone.ToString("F2");
                            s += "\n";
                        }
                    }
                }
                text.text = s;
            }
            else
            {
                text.text = "";
            }
        }
    }
}

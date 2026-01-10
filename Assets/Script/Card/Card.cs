using UnityEngine;

namespace CardProject
{
    public class Card : MonoBehaviour
    {
        ActionList actionList = new ActionList();
        
        public void InitCard(Vector2 destina)
        {
            actionList.AddAction(new MoveAction(false, 2.0f, 20.0f, destina));
            actionList.AddAction(new RotateAction(false,true,  2.0f, 500.0f, 5.0f,true));
            actionList.InitActionList(this.gameObject);
        }
        
        public void RunCardActions()
        {
            actionList.RunActions();
        }
    }
}

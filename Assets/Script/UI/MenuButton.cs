using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardProject
{
    public class MenuButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TextMeshProUGUI tmp;
        [SerializeField] private float buttonOffset;
        [SerializeField] private RectTransform rectTransform;
        
        public System.Action _onClick;
        public System.Action _onUp;

        public void SetButtonText(string text)
        {
            
            rectTransform.sizeDelta = new Vector2(40 + text.Length * buttonOffset, 60);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _onUp?.Invoke();
        }
    }
}

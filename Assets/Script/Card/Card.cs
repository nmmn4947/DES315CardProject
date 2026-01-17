using System;
using UnityEngine;

namespace CardProject
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer backCardRenderer;
        private bool isFront = true;

        private void Start()
        {
            
        }

        private void Update()
        {
            bool flipThreshHold = gameObject.transform.localRotation.eulerAngles.y%360.0f > 90.0f;
            if (flipThreshHold)
            {
                if (backCardRenderer.sortingOrder == 0)
                {
                    backCardRenderer.sortingOrder = 2;
                    isFront = false;
                }
                
            }else
            {
                backCardRenderer.sortingOrder = 0;
                isFront = true;
            }
        }
    }
}

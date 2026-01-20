using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public struct CardData
    {
        public enum SuitType
        {
            Clover,
            Diamond,
            Spade,
            Heart
        }
        public SuitType suit;
        public int number;
    }
    public class Card : MonoBehaviour
    {
        [SerializeField] private List<Sprite> FrontCardAssets = new List<Sprite>();
        [SerializeField] private SpriteRenderer frontCardRenderer;
        [SerializeField] private SpriteRenderer backCardRenderer;
        private bool isFront;
        
        CardData cardData;
        
        private void Update()
        {
            bool isFront = transform.localRotation.eulerAngles.y < 90f
                           || transform.localRotation.eulerAngles.y > 270f;
            frontCardRenderer.sortingOrder = isFront ? 1 : 0;
            backCardRenderer.sortingOrder  = isFront ? 0 : 1;
        }

        // i can only be 0 - 51
        public void SetCardData(int i)
        {
            if (i < 13)
            {
                cardData.suit = CardData.SuitType.Clover;
            }
            else if(i < 26){
                cardData.suit = CardData.SuitType.Diamond;
            }
            else if (i < 39)
            {
                cardData.suit = CardData.SuitType.Spade;
            }
            else
            {
                cardData.suit = CardData.SuitType.Heart;
            }
            
            cardData.number = i%13; //A - K : 12 == K

            frontCardRenderer.sprite = FrontCardAssets[i];
        }
    }
}

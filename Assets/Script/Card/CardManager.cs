using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace CardProject
{
    public class CardManager : MonoBehaviour
    {
        private ActionList actionList = new ActionList();
        
        public Card cardPrefab;
        
        private List<Card> cards = new List<Card>();
        
        private void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                Card currentCard = SpawnCard();
                //currentCard.InitCard(RandomPosition());
                cards.Add(currentCard);
            }
        }

        private void Update()
        {
            AddActionIfListEmpty();
            actionList.RunActions(Time.deltaTime);
        }

        private Card SpawnCard()
        {
            GameObject card1 = Instantiate(cardPrefab.gameObject);
            Card c1 = card1.GetComponent<Card>();
            return c1;
        }

        private Vector2 RandomPosition()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("No camera found");
                return Vector2.zero;
            }
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;
            
            float leftX = -halfWidth;
            float rightX = halfWidth;
            
            float bottomY = -halfHeight;
            float topY = halfHeight;
            
            return new Vector2(UnityEngine.Random.Range(leftX, rightX), UnityEngine.Random.Range(bottomY, topY));
        }

        private void AddAllCardsRandomSpinningScalingFlippingMovement()
        {
            float delay = 0.5f;
            for (int i = 0; i < cards.Count - 1; i++)
            {
                actionList.AddAction(new MoveAction(cards[i].gameObject, false, i * delay, 20.0f, RandomPosition()));
                
                bool isRight = i%2 != 0;
                actionList.AddAction(new RotateAction(cards[i].gameObject,false, i * delay, 500.0f,3.0f, isRight));
                
                actionList.AddAction(new FlipAction(cards[i].gameObject,false, 0.0f, 0.2f));
                
                Vector2 finalScale = new Vector2(1.0f, 1.0f);
                if (cards[i].gameObject.transform.localScale.x <= 1.05f)
                {
                    finalScale = new Vector2(3.0f, 3.0f);
                }
                actionList.AddAction(new ScaleAction(cards[i].gameObject,false, i * delay, finalScale, 3.0f));
            }

            int lastIndex = cards.Count - 1;
            
            bool right = (lastIndex)%2 != 0;
            actionList.AddAction(new MoveAction(cards[^1].gameObject,false, lastIndex * delay, 20.0f,RandomPosition()));
            actionList.AddAction(new RotateAction(cards[^1].gameObject,false, lastIndex * delay, 500.0f,3.0f, right));
            actionList.AddAction(new FlipAction(cards[^1].gameObject,false, 0.0f, 0.2f));
            Vector2 finalScale2 = new Vector2(1.0f, 1.0f);
            if (cards[^1].gameObject.transform.localScale.x <= 1.05f)
            {
                finalScale2 = new Vector2(3.0f, 3.0f);
            }
            actionList.AddAction(new ScaleAction(cards[^1].gameObject,true, lastIndex * delay, finalScale2, 3.0f));
        }

        private void AddActionIfListEmpty()
        {
            if (actionList.IsEmpty())
            {
                AddAllCardsRandomSpinningScalingFlippingMovement();
            }
        }
    }
}


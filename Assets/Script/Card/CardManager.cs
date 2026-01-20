using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

namespace CardProject
{
    public class CardManager : MonoBehaviour
    {
        public Card cardPrefab;
        
        private List<Card> cards = new List<Card>();
        private ActionList actionList = new ActionList();
        private float delay = 0.5f;
        
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
            Debug.Log(actionList.GetActionListCount());
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
            List<Action> newNestList = new List<Action>();
            for (int i = 0; i < cards.Count; i++)
            {
                AddOneLoop(newNestList, i); // all cards 
            }
            
            NestedAction allOfTheAbove = new NestedAction(newNestList, true, 0.0f);
            actionList.AddAction(allOfTheAbove);
            actionList.AddAction(new WaitAction(1.5f));
        }

        private void AddOneLoop(List<Action> list, int i)
        {
            bool isRight = i%2 != 0;
            MoveAction moveAction = new MoveAction(cards[i].gameObject, false, i * delay, 20.0f, RandomPosition());
            RotateAction rotateAction = new RotateAction(cards[i].gameObject,false, i * delay, 500.0f,float.MaxValue, isRight);
            Action.SynchronizeDurationFirstToSecond(moveAction, rotateAction);
            list.Add(moveAction);
            list.Add(rotateAction);
                
            list.Add(new FlipAction(cards[i].gameObject,false, 0.0f, 0.2f));
                
            Vector2 finalScale = new Vector2(1.0f, 1.0f);
            if (cards[i].gameObject.transform.localScale.x <= 1.05f)
            {
                finalScale = new Vector2(3.0f, 3.0f);
            }

            ScaleAction scaleAction = new ScaleAction(cards[i].gameObject, false, i * delay, finalScale, 3.0f);
            list.Add(scaleAction);
            Action.SynchronizeDurationFirstToSecond(moveAction, scaleAction);
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


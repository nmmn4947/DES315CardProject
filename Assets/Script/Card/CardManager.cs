using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace CardProject
{
    public class CardManager : MonoBehaviour
    {
        public Card cardPrefab;
        private List<Card> cards = new List<Card>();
        private void Start()
        {
            for (int i = 0; i < 2; i++)
            {
                Card currentCard = SpawnCard();
                currentCard.InitCard(RandomPosition());
                cards.Add(currentCard);
            }
        }

        private void Update()
        {
            foreach (Card card in cards)
            {
                card.RunCardActions();
            }
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
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;
            
            float leftX = -halfWidth;
            float rightX = halfWidth;
            
            float bottomY = -halfHeight;
            float topY = halfHeight;
            
            return new Vector2(UnityEngine.Random.Range(leftX, rightX), UnityEngine.Random.Range(bottomY, topY));
        }
    }
}


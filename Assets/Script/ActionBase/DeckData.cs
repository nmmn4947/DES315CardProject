using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class DeckData
    {
        public List<Card> cards = new List<Card>(); // THIS SHOULD WORK AS A STACK, LAST IN FIRST OUT
        public Vector3 currentPosition;

        public DeckData(Vector3 currentPosition, DeckHoldType currentHoldType, bool isFlipDown)
        {
            this.currentPosition = currentPosition;
            this.currentHoldType = currentHoldType;
            this.isFlipDown = isFlipDown;
        }

        public enum DeckHoldType
        {
            Stacked,
            Spread,
            None
        }
        public DeckHoldType currentHoldType;

        public bool isFlipDown;
        
        //Spread Deck Variable
        public float SpreadWidth = 80.0f;

        public Vector3 SpreadCardPosXCalculation(int index)
        {
            if (cards.Count == 1)
            {
                return currentPosition;
            }
            
            float halfSpread = SpreadWidth / 2f;
            float theOffset = halfSpread / cards.Count;

            Vector3 finalPos = currentPosition + (Vector3.right * theOffset * (index - cards.Count / 2f));
            finalPos = new Vector3(finalPos.x, finalPos.y, -0.1f * index); 
            
            return finalPos;
        }

        #region FakeStack

        public void Push(Card card)
        {
            cards.Add(card);
        }

        public void Pop()
        {
            if (cards.Count == 0) return;
            cards.RemoveAt(cards.Count - 1);
        }

        public Card Peek()
        {
            if (cards.Count == 0) return null;
            return cards[cards.Count - 1];
        }

        #endregion
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class DeckData
    {
        public List<Card> cards = new List<Card>(); // THIS SHOULD WORK AS A STACK, LAST IN FIRST OUT
        public Vector3 currentPosition;
        public float deckAngle;

        public DeckData(Vector3 currentPosition, DeckHoldType currentHoldType, bool isFlipDown, float deckRotation)
        {
            this.currentPosition = currentPosition;
            this.currentHoldType = currentHoldType;
            this.isFlipDown = isFlipDown;
            this.deckAngle = deckRotation;
        }

        public enum DeckHoldType
        {
            Stacked,
            Spread,
            UnorganizedStacked,
            None
        }
        public DeckHoldType currentHoldType;

        public bool isFlipDown;
        
        //Spread Deck Variable
        private float SpreadBorderWidth = 80.0f;
        //private float DEFAULTONECARDOFFSET = 7.68f;
        private float DEFAULTONECARDOFFSET = 6.5f;
        private float SpreaRange = 5.0f;

        public Vector3 SpreadCardPosXCalculation(int index, float angleDegrees)
        {
            if (cards.Count == 1)
            {
                return currentPosition;
            }
            
            float radians = angleDegrees * Mathf.Deg2Rad;
            
            Vector3 spreadDirection = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
            
            float halfSpread = SpreadBorderWidth / 2f;
            float theOffset = (halfSpread / cards.Count);
            
            //calculate proper offset
            if (DEFAULTONECARDOFFSET * (cards.Count - 1) <= SpreadBorderWidth)
            {
                theOffset = DEFAULTONECARDOFFSET;
            }

            Vector3 finalPos;
            if (cards.Count%2 == 0)
            {
                finalPos = currentPosition + spreadDirection * (theOffset * (index - (cards.Count / 2f) + 0.5f));
            }
            else
            {
                finalPos = currentPosition + spreadDirection * (theOffset * (index - (cards.Count / 2))); // no f

            }
            //Vector3 finalPos = currentPosition + Vector3.right * theOffset * index;
            finalPos = new Vector3(finalPos.x, finalPos.y, -0.1f * index);
            //Debug.Log( index + ":" + finalPos);
            
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

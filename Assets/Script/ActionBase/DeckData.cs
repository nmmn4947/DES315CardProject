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
        private float DEFAULTONECARDPOSOFFSET = 6.0f;
        private float DEFAULTONECARDROTOFFSET = 5.0f;
        private float SpreaRange = 5.0f;

        public Vector3 SpreadCardPosXCalculation(int index, float angleDegrees)
        {
            if (cards.Count == 1)
            {
                return currentPosition;
            }
            
            
            float radiansY = (angleDegrees - 90) * Mathf.Deg2Rad;
            float radians = (angleDegrees + 90) * Mathf.Deg2Rad;
            
            float side;

            if (angleDegrees == 90f)
            {
                side = Mathf.Sign(-Mathf.Cos(radians));
            }
            else
            {
                side = Mathf.Sign(Mathf.Cos(radians));
            }
            
            Vector3 spreadDirection = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
            Vector3 spreadDirectionY = new Vector3(-Mathf.Sin(radians), Mathf.Cos(radians), 0);

            if (angleDegrees == 180f)
            {
                spreadDirection = new Vector3(Mathf.Cos(radiansY), Mathf.Sin(radiansY), 0);
                spreadDirectionY = new Vector3(-Mathf.Sin(radiansY), Mathf.Cos(radiansY), 0);
            }

            //spreadDirection += spreadDirectionY * 0.1f;
            
            float halfSpread = SpreadBorderWidth / 2f;
            float theOffset = (halfSpread / cards.Count);
            
            //calculate proper offset
            if (DEFAULTONECARDPOSOFFSET * (cards.Count - 1) <= SpreadBorderWidth)
            {
                theOffset = DEFAULTONECARDPOSOFFSET;
            }

            //random
            float randomX = Random.Range(-0.25f, 0.25f);
            float randomY = Random.Range(-0.1f, 0.1f);
            
            theOffset += randomX;

            float arcStrength = 0.2f;
            arcStrength += randomY;
            
            Vector3 finalPos;
            if (cards.Count%2 == 0)
            {
                float t = (index - (cards.Count / 2f) + 0.5f);
                finalPos = currentPosition + spreadDirectionY * (theOffset * t) + spreadDirection * (t * t * arcStrength * side);
            }
            else
            {
                float t = (index - (cards.Count / 2));
                finalPos = currentPosition + spreadDirectionY * (theOffset * t) + spreadDirection * (t * t * arcStrength * side); 

            }
            //Vector3 finalPos = currentPosition + Vector3.right * theOffset * index;
            finalPos = new Vector3(finalPos.x, finalPos.y, 0.1f * index);
            //Debug.Log( index + ":" + finalPos);
            
            return finalPos;
        }
        
        public float SpreadOrganicRotateCalculation(int index, float angleDegrees, int cardsCount)
        {
            if (cardsCount == 1)
            {
                return angleDegrees;
            }

            float finalRotation;

            float randomR = Random.Range(-5f, 5f);
            
            if (cardsCount % 2 == 0)
            {
                finalRotation = angleDegrees + ((index - (cardsCount / 2f) + 0.5f) * DEFAULTONECARDROTOFFSET);
            }
            else
            {
                finalRotation = angleDegrees + ((index - (cardsCount / 2)) * DEFAULTONECARDROTOFFSET);
            }
            return finalRotation + randomR;
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

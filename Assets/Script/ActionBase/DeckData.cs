using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class DeckData
    {
        public List<Card> cards = new List<Card>();
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
    }
}

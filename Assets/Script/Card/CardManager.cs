using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<DeckData> deckData = new List<DeckData>();
        private DeckData freeDeck = new DeckData(Vector3.zero, DeckData.DeckHoldType.None, false);
        private DeckData drawDeck = new DeckData(Vector3.zero, DeckData.DeckHoldType.Stacked, true);
        private DeckData player1Hand;
        [SerializeField] private GameObject handPosition;
        
        private float DEFAULTDELAY = 0.01f;
        private float DEFAULTSTACKEDPOSOFFSET = 0.05f;
        private float DEFAULTSTACKEDZOFFSET = 0.1f;
        
        private void Start()
        {
            player1Hand = new DeckData(handPosition.transform.position, DeckData.DeckHoldType.Spread, false);
                
            //DeckData
            deckData.Add(freeDeck);
            deckData.Add(drawDeck);
            deckData.Add(player1Hand);
            
            //AllCardinit
            for (int i = 0; i < 52; i++)
            {
                Card currentCard = SpawnCard(i, RandomPosition());
                cards.Add(currentCard);
                freeDeck.cards.Add(currentCard);
            }

            MoveAllCardsInToThisDeck(drawDeck, DEFAULTDELAY * 5);
            ShuffleThisDeck(drawDeck);
            MoveSomeCardsIntoDeck(drawDeck, player1Hand, 7, DEFAULTDELAY * 5);
            //actionList.AddAction(new WaitAction(5f));
            //MoveSomeCardsIntoDeck(drawDeck, player1Hand, 7, DEFAULTDELAY);
        }

        private void Update()
        {
            //AddActionIfListEmpty();
            CardUpdateOnHand(player1Hand);
            actionList.RunActions(Time.deltaTime);
        }
        
        #region SpawningCard

        private Card SpawnCard(int i)
        {
            GameObject card1 = Instantiate(cardPrefab.gameObject);
            Card c1 = card1.GetComponent<Card>();
            c1.SetCardData(i);
            return c1;
        }
        private Card SpawnCard(int i, Vector3 startPosition)
        {
            GameObject card1 = Instantiate(cardPrefab.gameObject, startPosition, Quaternion.identity);
            Card c1 = card1.GetComponent<Card>();
            c1.SetCardData(i);
            return c1;
        }

        #endregion

        #region TestingCode
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
            MoveAction moveAction = new MoveAction(cards[i].gameObject, false, i * 0.5f, 20.0f, RandomPosition());
            SpinAction rotateAction = new SpinAction(cards[i].gameObject,false, i * 0.5f, 500.0f,float.MaxValue, isRight);
            Action.SynchronizeDurationFirstToSecond(moveAction, rotateAction);
            list.Add(moveAction);
            list.Add(rotateAction);
                
            list.Add(new FlipAction(cards[i].gameObject,false, 0.0f, 0.2f));
                
            Vector2 finalScale = new Vector2(1.0f, 1.0f);
            if (cards[i].gameObject.transform.localScale.x <= 1.05f)
            {
                finalScale = new Vector2(3.0f, 3.0f);
            }

            ScaleAction scaleAction = new ScaleAction(cards[i].gameObject, false, i * 0.5f, finalScale, 3.0f);
            list.Add(scaleAction);
            Action.SynchronizeDurationFirstToSecond(moveAction, scaleAction);   
        }
        #endregion
        
        private void AddActionIfListEmpty()
        {
            if (actionList.IsEmpty())
            {
                actionList.AddAction(new WaitAction(3f));
                CombineCardToDeck(drawDeck.currentPosition);
                actionList.AddAction(new WaitAction(1f));
                RotateAllCardsStairCaseOnce(0.01f);
            }
        }
        
        #region CardLogic
        
        public void MoveAllCardsInToThisDeck(DeckData chosendeck, float delayEachCard)
        {
            //MOVING FROM ALL DECKS
            foreach (DeckData dd in deckData)
            {
                if (dd == chosendeck)
                {
                    continue;
                }
                
                int originalNotChosenDeckSize = dd.cards.Count;
                
                List<Action> newNestList = new List<Action>();
                int i = 0;
                while (dd.cards.Count > 0)
                {
                    chosendeck.cards.Insert(0,dd.cards[0]);
                    MoveAction moveAction = null;
                    switch (chosendeck.currentHoldType)
                    {
                        case DeckData.DeckHoldType.Stacked:
                            moveAction = new MoveAction(0.25f, dd.cards[0].gameObject, false, delayEachCard * i, PosMoveCardStackedStyle(i, chosendeck.currentPosition, originalNotChosenDeckSize));
                            newNestList.Add(moveAction);
                            break;
                        case DeckData.DeckHoldType.Spread:
                            MoveSpreadDeck(ref newNestList, chosendeck, delayEachCard, i);
                            break;
                        case DeckData.DeckHoldType.None:
                            break;
                    }
                    if (chosendeck.isFlipDown)
                    {
                        if (!dd.cards[0].GetIsFront())
                        {
                            newNestList.Add(new FlipAction(dd.cards[0].gameObject, false, delayEachCard * i, 0.3f));
                        }
                    }
                    else
                    {
                        if (dd.cards[0].GetIsFront())
                        {
                            newNestList.Add(new FlipAction(dd.cards[0].gameObject, false, delayEachCard * i, 0.3f));
                        }
                    }

                    i++;
                    dd.cards.RemoveAt(0);
                }
                NestedAction eachDeckMove = new NestedAction(newNestList, true, 0.0f);
                actionList.AddAction(eachDeckMove);
            }
        }
        
        public void MoveSomeCardsIntoDeck(DeckData moveFromDeck, DeckData moveToDeck, int amountOfCards, float delayEachCard)
        {
            if (amountOfCards > moveFromDeck.cards.Count)
            {
                Debug.LogError("Amount of cards is more than Deck");
                //Or draw nonetheless?
                return;
            }
            
            //WRONG LOGIC RIGHT NOW
            int originalNotChosenDeckSize = moveFromDeck.cards.Count;
                
            List<Action> newNestList = new List<Action>();
            for (int i = 0; i < amountOfCards; i++)
            {
                moveToDeck.cards.Insert(0,moveFromDeck.cards[0]);
                MoveAction moveAction = null;
                switch (moveToDeck.currentHoldType)
                {
                    case DeckData.DeckHoldType.Stacked:
                        moveAction = new MoveAction(0.25f, moveFromDeck.cards[0].gameObject, false, delayEachCard * i, PosMoveCardStackedStyle(i, moveToDeck.currentPosition, originalNotChosenDeckSize));
                        newNestList.Add(moveAction);
                        break;
                    case DeckData.DeckHoldType.Spread:
                        MoveSpreadDeck(ref newNestList,moveToDeck, delayEachCard, i);
                        break;
                    case DeckData.DeckHoldType.None:
                        break;
                }
                if (moveToDeck.isFlipDown)
                {
                    if (moveFromDeck.cards[0].GetIsFront())
                    {
                        newNestList.Add(new FlipAction(moveFromDeck.cards[0].gameObject, false, delayEachCard * i, 0.3f));
                    }
                }
                else
                {
                    if (!moveFromDeck.cards[0].GetIsFront())
                    {
                        newNestList.Add(new FlipAction(moveFromDeck.cards[0].gameObject, false, delayEachCard * i, 0.3f));
                    }
                }
                moveFromDeck.cards.RemoveAt(0);
            }
            
            NestedAction nestedList = new NestedAction(newNestList, true, 0.0f);
            actionList.AddAction(nestedList);
        }

        public void ShuffleThisDeck(DeckData chosendeck)
        {
            switch (chosendeck.currentHoldType)
            {
                case DeckData.DeckHoldType.Stacked:
                    ShuffleStackedDeck(chosendeck);
                    break;
                case DeckData.DeckHoldType.Spread:
                    break;
                case DeckData.DeckHoldType.None:
                    break;
            }
        }

        private void ShuffleStackedDeck(DeckData chosendeck)
        {
            System.Random rng = new System.Random();

            int n = chosendeck.cards.Count;
            int i = 0;
            List<Action> newNestList = new List<Action>();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (chosendeck.cards[k], chosendeck.cards[n]) = (chosendeck.cards[n], chosendeck.cards[k]); //swap
                newNestList.Add(new RotateAction(chosendeck.cards[n].gameObject, false, i * DEFAULTDELAY * 1.5f, 0.5f, 0, 1, false));
                newNestList.Add(new RotateAction(chosendeck.cards[k].gameObject, false, i * DEFAULTDELAY * 1.5f, 0.5f, 0, 1, true));
                i++;
            }
            NestedAction nested = new NestedAction(newNestList, true, 0.0f);
            actionList.AddAction(nested);
        }
        
        private Vector3 PosMoveCardStackedStyle(int i, Vector3 starterDeck, int originalDeckSize)
        {
            int pos = originalDeckSize - i;
            return starterDeck - new Vector3(pos * DEFAULTSTACKEDPOSOFFSET, pos * DEFAULTSTACKEDPOSOFFSET, i * DEFAULTSTACKEDZOFFSET);
        }
        
        private void MoveSpreadDeck(ref List<Action> actions, DeckData chosendeck, float delayEachCard, int index)
        {
            //List<Action> newNestList = new List<Action>();
            for (int i = 0; i < chosendeck.cards.Count; i++)
            {
                //newNestList.Add(new MoveAction(0.25f, moveSubject, false, delayEachCard * index, chosendeck.currentPosition + Vector3.right * chosendeck.SpreadCardPosXCalculation(index)));
                actions.Add(new MoveAction(0.25f, chosendeck.cards[i].gameObject, false, delayEachCard * index, chosendeck.SpreadCardPosXCalculation(i)));
            }
            //NestedAction nested = new NestedAction(newNestList, true, 0.0f);
            //actionList.AddAction(nested);
        }

        private void CardUpdateOnHand(DeckData data)
        {
            
        }
        #endregion
        
        #region MoveCardStuff

        public void CombineCardToDeck(Vector3 deckPosition)
        {
            for (int i = 0; i < 52; i++)
            {
                EachCardActionAddCombineDeck(i, deckPosition,0.05f);
            }
        }
        
        private void EachCardActionAddCombineDeck(int i, Vector3 deckPosition, float positionOffset)
        {
            float indexOffset = positionOffset * i;
            float zIndex = 0.1f * i;
            FlipAction flipAction = new FlipAction(cards[i].gameObject,false, 0.0f, 0.2f);
            MoveAction moveAction = new MoveAction(0.5f, cards[i].gameObject, false, 0.0f, new Vector3(deckPosition.x - indexOffset, deckPosition.y - indexOffset,deckPosition.z + zIndex));

            actionList.AddAction(flipAction);
            actionList.AddAction(moveAction);
        }
        
        public void RotateAllCardsStairCaseOnce(float eachdelay)
        {
            for (int i = 0; i < 52; i++)
            {
                RotateAction rotateAction = new RotateAction(cards[i].gameObject, false, i * eachdelay, 0.25f, 0.0f, 1, false);
                actionList.AddAction(rotateAction);
            }
        }
        
        public void RotateAllCardsSwitchEachOnce(float eachdelay)
        {
            for (int i = 0; i < 52; i++)
            {
                RotateAction rotateAction = new RotateAction(cards[i].gameObject, false, i * eachdelay, 0.25f, 0.0f, 1, i%2 == 0);
                actionList.AddAction(rotateAction);
            }
        }

        public void ShuffleTheDeck()
        {
            
        }
        
        #endregion

        
    }
}


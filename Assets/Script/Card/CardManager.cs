using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private DeckData drawDeck;
        private DeckData player1Hand;
        [SerializeField] private GameObject handPosition;
        [SerializeField] private GameObject drawDeckPosition;
        
        private float DEFAULTDELAY = 0.01f;
        private float DEFAULTSTACKEDPOSOFFSET = 0.05f;
        private float DEFAULTSTACKEDZOFFSET = 0.1f;
        private float DEFAULTFLIPDURATION = 0.6f;
        private float DEFAULTMOVEDURATION = 0.4f;
        
        private void Start()
        {
            drawDeck = new DeckData(drawDeckPosition.transform.position, DeckData.DeckHoldType.Stacked, true);
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

            MoveCardsIntoDeck(freeDeck, drawDeck, 52,DEFAULTDELAY * 5);
            actionList.AddAction(new CallBackAction(() => ShuffleThisDeck(drawDeck), true, 0.0f, 0.5f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(drawDeck, player1Hand, 7, DEFAULTDELAY * 5), true, 0.0f, 0.5f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(player1Hand, drawDeck, 7, DEFAULTDELAY * 5), true, 0.0f, 0.5f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(drawDeck, player1Hand, 7, DEFAULTDELAY * 5), true, 0.0f, 0.5f));
            /*System.Action act = () => ShuffleThisDeck(drawDeck);
            actionList.AddAction(new CallBackAction(act, true,0.0f, 0.5f ));
            System.Action act2 = () => MoveCardsIntoDeck(drawDeck, player1Hand, 52, DEFAULTDELAY * 5);
            actionList.AddAction(new CallBackAction(act2, true, 0.0f, 0.5f ));
            System.Action act3 = () => MoveCardsIntoDeck(player1Hand, drawDeck, 52, DEFAULTDELAY * 5);
            actionList.AddAction(new CallBackAction(act3, true, 0.0f, 0.5f ));
            System.Action act4 = () => MoveCardsIntoDeck(player1Hand, drawDeck, 52, DEFAULTDELAY * 5);
            actionList.AddAction(new CallBackAction(act4, true, 0.0f, 0.5f ));*/
            
            //actionList.AddAction(new WaitAction(5f));
            //MoveSomeCardsIntoDeck(drawDeck, player1Hand, 7, DEFAULTDELAY);
        }

        private void Update()
        {
            //AddActionIfListEmpty();
            CardUpdateOnHand(player1Hand);
            actionList.RunActions(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
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
        
        public void MoveCardsIntoDeck(DeckData moveFromDeck, DeckData moveToDeck, int amountOfCards, float delayEachCard)
        {
            if (amountOfCards > moveFromDeck.cards.Count)
            {
                Debug.LogError("Amount of cards is more than Deck");
                //Or draw nonetheless?
                return;
            }
            
            //WRONG LOGIC RIGHT NOW
            int originalNotChosenDeckSize = moveFromDeck.cards.Count;
            int originalChosenDeckSize = moveToDeck.cards.Count;
            //Debug.Log(moveFromDeck.Peek().transform.position.z);
            List<Action> newNestList = new List<Action>();
            for (int i = 0; i < amountOfCards; i++)
            {
                moveToDeck.Push(moveFromDeck.Peek());
                Card currentCard = moveFromDeck.Peek();
                MoveAction moveAction = null;
                switch (moveToDeck.currentHoldType)
                {
                    case DeckData.DeckHoldType.Stacked:
                        moveAction = new MoveAction(DEFAULTMOVEDURATION, currentCard.gameObject, false, delayEachCard * i, PosMoveCardStackedStyle(moveToDeck.cards.Count - 1, moveToDeck.currentPosition, originalNotChosenDeckSize));
                        newNestList.Add(moveAction);
                        break;
                    case DeckData.DeckHoldType.Spread:
                        MoveSpreadDeck(ref newNestList,moveToDeck, delayEachCard, moveToDeck.cards.Count - 1);
                        break;
                    case DeckData.DeckHoldType.None:
                        break;
                }
                if (moveToDeck.isFlipDown)
                {
                    if (!currentCard.GetIsFront())
                    {
                        newNestList.Add(new FlipAction(currentCard.gameObject, false, delayEachCard * i, DEFAULTFLIPDURATION));
                    }
                }
                else
                {
                    if (!currentCard.GetIsFront())
                    {
                        newNestList.Add(new FlipAction(currentCard.gameObject, false, delayEachCard * i, DEFAULTFLIPDURATION));
                    }
                }
                moveFromDeck.Pop();
                //Debug.Log(moveToDeck.cards.Count - 1);
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
                Vector3 posk = chosendeck.cards[k].transform.position;
                Vector3 posn = chosendeck.cards[n].transform.position;
                (chosendeck.cards[k], chosendeck.cards[n]) = (chosendeck.cards[n], chosendeck.cards[k]); //swap
                newNestList.Add(new RotateAction(chosendeck.cards[n].gameObject, false, i * DEFAULTDELAY * 1.5f, 0.5f, 0, 1, false));
                newNestList.Add(new MoveAction(0.1f, chosendeck.cards[n].gameObject, false, i * DEFAULTDELAY * 1.5f, posn)); // SWAP INSTANTLY
                newNestList.Add(new RotateAction(chosendeck.cards[k].gameObject, false, i * DEFAULTDELAY * 1.5f, 0.5f, 0, 1, true));
                newNestList.Add(new MoveAction(0.1f, chosendeck.cards[k].gameObject, false, i * DEFAULTDELAY * 1.5f, posk));
                i++;
            }
            
            List<Action> nNestList = new List<Action>();
            for (int j = 0; j < chosendeck.cards.Count; j++)
            {
                Vector3 correctPos = PosMoveCardStackedStyle(j, chosendeck.currentPosition, chosendeck.cards.Count);
                newNestList.Add(new MoveAction(0.1f, chosendeck.cards[j].gameObject, false, i * DEFAULTDELAY * 1.5f, correctPos));
            }
            NestedAction nested = new NestedAction(newNestList, true, 0.0f);
            //NestedAction nested2 = new NestedAction(nNestList, true, 0.0f);
            actionList.AddAction(nested);
            //actionList.AddAction(nested2);
        }
        
        private Vector3 PosMoveCardStackedStyle(int i, Vector3 starterDeck, int originalDeckSize)
        {
            //int pos = originalDeckSize - i;
            return starterDeck - new Vector3(-i * DEFAULTSTACKEDPOSOFFSET, -i * DEFAULTSTACKEDPOSOFFSET, i * DEFAULTSTACKEDZOFFSET);
        }
        
        private void MoveSpreadDeck(ref List<Action> actions, DeckData chosendeck, float delayEachCard, int index)
        {
            //List<Action> newNestList = new List<Action>();
            for (int i = 0; i < chosendeck.cards.Count; i++)
            {
                actions.Add(new MoveAction(DEFAULTMOVEDURATION, chosendeck.cards[i].gameObject, false, delayEachCard * index, chosendeck.SpreadCardPosXCalculation(i)));
            }
            //NestedAction nested = new NestedAction(newNestList, true, 0.0f);
            //actionList.AddAction(nested);
        }

        private void CardUpdateOnHand(DeckData data)
        {
            
        }

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
        

        
    }
}


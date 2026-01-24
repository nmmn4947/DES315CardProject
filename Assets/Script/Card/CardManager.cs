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
        private DeckData freeDeck = new DeckData(Vector3.zero, DeckData.DeckHoldType.None, false, 0);
        private DeckData playDeck;
        private DeckData drawDeck;
        private DeckData discardDeck;
        private DeckData player1Hand;
        private DeckData player2Hand;
        private DeckData player3Hand;
        private DeckData player4Hand;
        private Card currentHoverCard;
        
        [SerializeField] private GameObject hand1Position;
        [SerializeField] private GameObject hand2Position;
        [SerializeField] private GameObject hand3Position;
        [SerializeField] private GameObject hand4Position;
        [SerializeField] private GameObject drawDeckPosition;
        [SerializeField] private GameObject playDeckPosition;
        [SerializeField] private GameObject discardDeckPosition;
        
        private float DEFAULTDELAY = 0.01f;
        private float DEFAULTSTACKEDPOSOFFSET = 0.05f;
        private float DEFAULTSTACKEDZOFFSET = 0.1f;
        private float DEFAULTFLIPDURATION = 0.5f;
        private float DEFAULTMOVEDURATION = 0.25f;
        
        private void Start()
        {
            playDeck = new DeckData(playDeckPosition.transform.position, DeckData.DeckHoldType.Stacked, false, 0);
            drawDeck = new DeckData(drawDeckPosition.transform.position, DeckData.DeckHoldType.Stacked, true, 0);
            discardDeck = new DeckData(discardDeckPosition.transform.position, DeckData.DeckHoldType.UnorganizedStacked, false, 67);
            player1Hand = new DeckData(hand1Position.transform.position, DeckData.DeckHoldType.Spread, false, 0);
            player2Hand = new DeckData(hand2Position.transform.position, DeckData.DeckHoldType.Spread, true, -90);
            player3Hand = new DeckData(hand3Position.transform.position, DeckData.DeckHoldType.Spread, true, 180);
            player4Hand = new DeckData(hand4Position.transform.position, DeckData.DeckHoldType.Spread, true, 90);
                
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

            MoveCardsIntoDeck(freeDeck, drawDeck, 52,DEFAULTDELAY * 5, true);
            actionList.AddAction(new CallBackAction(() => ShuffleThisDeck(drawDeck), true, 0.0f, 0.5f));
            actionList.AddAction(new WaitAction(0.5f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(drawDeck, player1Hand, 7, DEFAULTDELAY * 5, true), true, 0.0f, 0.5f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(drawDeck, player2Hand, 7, DEFAULTDELAY * 5, true), true, 0.0f, 0.5f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(drawDeck, player3Hand, 7, DEFAULTDELAY * 5, true), true, 0.0f, 0.5f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(drawDeck, player4Hand, 7, DEFAULTDELAY * 5, true), true, 0.0f, 0.5f));
            //actionList.AddAction(new WaitAction(5f));
            //MoveSomeCardsIntoDeck(drawDeck, player1Hand, 7, DEFAULTDELAY);
        }

        private void Update()
        {
            CardUpdateOnHand(player1Hand);
            actionList.RunActions(Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                MoveCardsIntoDeck(discardDeck, drawDeck, discardDeck.cards.Count,DEFAULTDELAY * 5, true);
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
        
        public void MoveCardsIntoDeck(DeckData moveFromDeck, DeckData moveToDeck, int amountOfCards, float delayEachCard, bool willRotate)
        {
            if (amountOfCards > moveFromDeck.cards.Count)
            {
                Debug.LogError("Amount of cards is more than Deck");
                //Or draw nonetheless?
                return;
            }
            
            int originalNotChosenDeckSize = moveFromDeck.cards.Count;
            List<Action> newNestList = new List<Action>();
            for (int i = 0; i < amountOfCards; i++)
            {
                //Add card to list and move it
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
                        MoveCardIntoSpreadDeck(ref newNestList,moveToDeck, delayEachCard, moveToDeck.cards.Count - 1);
                        break;
                    case DeckData.DeckHoldType.UnorganizedStacked:
                        moveAction = new MoveAction(DEFAULTMOVEDURATION, currentCard.gameObject, false, delayEachCard * i, 
                            PosMoveCardStackedStyle(moveToDeck.cards.Count - 1, moveToDeck.currentPosition, originalNotChosenDeckSize) + (Vector3.right * UnityEngine.Random.Range(-1.0f, 1.0f)) + (Vector3.up * UnityEngine.Random.Range(-1.0f, 1.0f)));
                        newNestList.Add(moveAction);
                        break;
                    case DeckData.DeckHoldType.None:
                        break;
                }
                
                if (moveToDeck.isFlipDown != moveFromDeck.isFlipDown)
                {
                    newNestList.Add(new FlipAction(currentCard.gameObject, false, delayEachCard * i, DEFAULTFLIPDURATION));
                }

                if (willRotate)
                {
                    switch (moveToDeck.currentHoldType)
                    {
                        case DeckData.DeckHoldType.Stacked:
                            newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * i, 1.0f, moveToDeck.deckAngle, 1, true));
                            break;
                        case DeckData.DeckHoldType.Spread:
                            newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * i, 1.0f, moveToDeck.deckAngle, 1, true));
                            break;
                        case DeckData.DeckHoldType.UnorganizedStacked:
                            newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * i, 1.0f, UnityEngine.Random.Range(0.0f, 360.0f), 1, true));
                            break;
                        case DeckData.DeckHoldType.None:
                            break;
                    }
                }

                //remove it from the list
                moveFromDeck.Pop();
            }
            
            //Adjust the movefrom deck if needed AFTER REMOVE ALL MOVING CARDS from list
            switch (moveFromDeck.currentHoldType)
            {
                case DeckData.DeckHoldType.Stacked:
                    //No need
                    break;
                case DeckData.DeckHoldType.Spread:
                    Debug.Log(moveFromDeck.cards.Count);
                    AdjustSpreadDeck(ref newNestList, moveFromDeck, delayEachCard);
                    break;
                case DeckData.DeckHoldType.UnorganizedStacked:
                    break;
                case DeckData.DeckHoldType.None:
                        
                    break;
            }
            
            NestedAction nestedList = new NestedAction(newNestList, true, 0.0f);
            actionList.AddAction(nestedList);
        }
        
        public void SelectCardsIntoDeck(DeckData moveFromDeck, Card selectedCard, DeckData moveToDeck, float delayEachCard, bool willRotate)
        {
            int theIndex = -1;
            for (int i = 0; i < moveFromDeck.cards.Count; i++) 
            {
                if (selectedCard.cardData.suit == moveFromDeck.cards[i].cardData.suit && selectedCard.cardData.number == moveFromDeck.cards[i].cardData.number)
                {
                    theIndex = i;
                    break;
                }
            }

            if (theIndex == -1)
            {
                Debug.LogError("No Card Selected");
                return;
            }
            
            int originalNotChosenDeckSize = moveFromDeck.cards.Count;
            
            List<Action> newNestList = new List<Action>();

            //Add card to list and move it
            Card currentCard = moveFromDeck.cards[theIndex];
            moveToDeck.Push(currentCard);
            
            MoveAction moveAction = null;
            switch (moveToDeck.currentHoldType)
            {
                case DeckData.DeckHoldType.Stacked:
                    moveAction = new MoveAction(DEFAULTMOVEDURATION, currentCard.gameObject, false, delayEachCard * theIndex, PosMoveCardStackedStyle(moveToDeck.cards.Count - 1, moveToDeck.currentPosition, originalNotChosenDeckSize));
                    newNestList.Add(moveAction);
                    break;
                case DeckData.DeckHoldType.Spread:
                    MoveCardIntoSpreadDeck(ref newNestList,moveToDeck, delayEachCard, moveToDeck.cards.Count - 1);
                    break;
                case DeckData.DeckHoldType.UnorganizedStacked:
                    moveAction = new MoveAction(DEFAULTMOVEDURATION, currentCard.gameObject, false, delayEachCard * theIndex, 
                        PosMoveCardStackedStyle(moveToDeck.cards.Count - 1, moveToDeck.currentPosition, originalNotChosenDeckSize) + (Vector3.right * UnityEngine.Random.Range(-1.0f, 1.0f)) + (Vector3.up * UnityEngine.Random.Range(-1.0f, 1.0f)));
                    newNestList.Add(moveAction);
                    break;
                case DeckData.DeckHoldType.None:
                    break;
            }
            //If it needs to flip or not
            if (moveToDeck.isFlipDown != moveFromDeck.isFlipDown) // Deck check, not good, card check good
            {
                newNestList.Add(new FlipAction(currentCard.gameObject, false, delayEachCard * theIndex, DEFAULTFLIPDURATION));
            }

            if (willRotate)
            {
                switch (moveToDeck.currentHoldType)
                {
                    case DeckData.DeckHoldType.Stacked:
                        newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * theIndex, 1.0f,
                            moveToDeck.deckAngle, 1, true));
                        break;
                    case DeckData.DeckHoldType.Spread:
                        newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * theIndex, 1.0f,
                            moveToDeck.deckAngle, 1, true));
                        break;
                    case DeckData.DeckHoldType.UnorganizedStacked:
                        newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * theIndex, 1.0f,
                            UnityEngine.Random.Range(0.0f, 360.0f), 1, true));
                        break;
                    case DeckData.DeckHoldType.None:
                        break;
                }
            }

            //remove it from the list
            moveFromDeck.cards.RemoveAt(theIndex);
            
            
            //Adjust the movefrom deck if needed AFTER REMOVE ALL MOVING CARDS from list
            switch (moveFromDeck.currentHoldType)
            {
                case DeckData.DeckHoldType.Stacked:
                    //No need
                    break;
                case DeckData.DeckHoldType.Spread:
                    Debug.Log(moveFromDeck.cards.Count);
                    AdjustSpreadDeck(ref newNestList, moveFromDeck, delayEachCard);
                    break;
                case DeckData.DeckHoldType.UnorganizedStacked:
                    break;
                case DeckData.DeckHoldType.None:
                        
                    break;
            }
            
            NestedAction nestedList = new NestedAction(newNestList, true, 0.0f);
            actionList.AddAction(nestedList);
        }

        private void MoveAnIndexOfCardIntoDeck(DeckData moveFromDeck, int index, DeckData moveToDeck, float delayEachCard, bool willRotate)
        {
            SelectCardsIntoDeck(moveFromDeck,moveFromDeck.cards[index], moveToDeck, delayEachCard, willRotate);
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
                case DeckData.DeckHoldType.UnorganizedStacked:
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
                newNestList.Add(new RotateAction(chosendeck.cards[n].gameObject, false, i * DEFAULTDELAY * 1.5f, 0.5f, 0f, 1, false));
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
        
        /*private void ShuffleStackedDeck(DeckData chosendeck)
        {
            System.Random rng = new System.Random();

            int n = chosendeck.cards.Count;
            List<Action> newNestList = new List<Action>();
            List<Action> newNestList2 = new List<Action>();

            int a = 0;

            for (int i = chosendeck.cards.Count-1 ; i >= 0; i--)
            {
                
                newNestList.Add(new RotateAction(chosendeck.cards[i].gameObject, false, a * DEFAULTDELAY * 1.5f, 0.8f, UnityEngine.Random.Range(0f, 360f), 3, false));
                /*newNestList.Add(new MoveAction(0.5f, chosendeck.cards[n].gameObject, false,  i * DEFAULTDELAY * 1.5f,
                    chosendeck.currentPosition + new Vector3(UnityEngine.Random.Range(10f, 15f), UnityEngine.Random.Range(-10f, 10f), posn.z)));#1#
                newNestList.Add(new MoveAction(0.5f, chosendeck.cards[i].gameObject, false, a * DEFAULTDELAY * 1.5f, RandomPosition()));
                a++;
            }
            
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Vector3 posk = chosendeck.cards[k].transform.position;
                Vector3 posn = chosendeck.cards[n].transform.position;
                (chosendeck.cards[k], chosendeck.cards[n]) = (chosendeck.cards[n], chosendeck.cards[k]);
            }
            for (int j = 0; j < chosendeck.cards.Count; j++)
            {
                Vector3 correctPos = PosMoveCardStackedStyle(j, chosendeck.currentPosition, chosendeck.cards.Count);
                newNestList2.Add(new MoveAction(0.5f, chosendeck.cards[j].gameObject, false, (j) * DEFAULTDELAY * 1.5f, correctPos));
                newNestList2.Add(new RotateAction(chosendeck.cards[j].gameObject, false, (j) * DEFAULTDELAY * 1.5f, 0.8f, 0, 5, true));
            }
            NestedAction nested = new NestedAction(newNestList, true, 0.0f);
            NestedAction nested2 = new NestedAction(newNestList2, true, 0.0f);
            actionList.AddAction(nested);
            actionList.AddAction(nested2);
        }*/
        
        private Vector3 PosMoveCardStackedStyle(int i, Vector3 starterDeck, int originalDeckSize)
        {
            //int pos = originalDeckSize - i;
            return starterDeck - new Vector3(-i * DEFAULTSTACKEDPOSOFFSET, -i * DEFAULTSTACKEDPOSOFFSET, i * DEFAULTSTACKEDZOFFSET);
        }
        
        private void MoveCardIntoSpreadDeck(ref List<Action> actions, DeckData chosendeck, float delayEachCard, int index)
        {
            //List<Action> newNestList = new List<Action>();
            for (int i = 0; i < chosendeck.cards.Count; i++)
            {
                actions.Add(new MoveAction(DEFAULTMOVEDURATION, chosendeck.cards[i].gameObject, false, delayEachCard * index, chosendeck.SpreadCardPosXCalculation(i, chosendeck.deckAngle)));
            }
            chosendeck.cards[^1].deckZ = chosendeck.SpreadCardPosXCalculation(chosendeck.cards.Count - 1, chosendeck.deckAngle).z;
        }

        private void AdjustSpreadDeck(ref List<Action> actions, DeckData chosendeck, float delayEachCard)
        {
            if (chosendeck.currentHoldType != DeckData.DeckHoldType.Spread)
            {
                Debug.LogError("Deck is not Spread");
                return;
            }
            
            for (int i = 0; i < chosendeck.cards.Count; i++)
            {
                actions.Add(new MoveAction(DEFAULTMOVEDURATION, chosendeck.cards[i].gameObject, false, delayEachCard * i, chosendeck.SpreadCardPosXCalculation(i, chosendeck.deckAngle)));
            }
        }

        private void CardUpdateOnHand(DeckData data)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
    
            Card newHoverCard = null;
    
            // Check if we hit a card
            if (raycastHit2D.collider != null)
            {
                Card hitCard = raycastHit2D.collider.gameObject.GetComponent<Card>();
        
                // Only consider it if the card is in the player's hand
                if (hitCard != null && data.cards.Contains(hitCard))
                {
                    newHoverCard = hitCard;
                }
            }
    
            // If we're hovering over a different card than before
            if (newHoverCard != currentHoverCard)
            {
                // Scale down the previous card if there was one
                if (currentHoverCard != null)
                {
                    actionList.AddAction(new ScaleAction(currentHoverCard.gameObject, false, 0.0f, new Vector2(1.0f, 1.0f), 0.25f));
                }
        
                // Scale up the new card if there is one
                if (newHoverCard != null)
                {
                    actionList.AddAction(new ScaleAction(newHoverCard.gameObject, false, 0.0f, new Vector2(1.2f, 1.2f), 0.25f));
                }
        
                // Update current hover card
                currentHoverCard = newHoverCard;
            }
    
            // Handle mouse clicks on cards
            /*if (actionList.IsEmpty())
            {
                return;
            }*/
            if (currentHoverCard != null && Input.GetMouseButtonDown(0))
            {
                // Do something when clicking on a card
                SelectCardsIntoDeck(player1Hand, currentHoverCard, playDeck, 0.0f, false);
                //CHANGE THIS TO DEAL FOR EACH PLAYER 1 CARD AT A TIME
                actionList.AddAction(new CallBackAction(() => MoveAnIndexOfCardIntoDeck(player2Hand, UnityEngine.Random.Range(0, player2Hand.cards.Count), playDeck, 0.0f, false), true, 0.0f, 1.0f));
                actionList.AddAction(new CallBackAction(() => MoveAnIndexOfCardIntoDeck(player3Hand, UnityEngine.Random.Range(0, player2Hand.cards.Count), playDeck, 0.0f, false), true, 0.0f, 1.0f));
                actionList.AddAction(new CallBackAction(() => MoveAnIndexOfCardIntoDeck(player4Hand, UnityEngine.Random.Range(0, player2Hand.cards.Count), playDeck, 0.0f, false), true, 0.0f, 1.0f));
                actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(playDeck, discardDeck, playDeck.cards.Count, 0.0f, true), true, 0.0f, 0.5f));
            }
        }
        
    }
}


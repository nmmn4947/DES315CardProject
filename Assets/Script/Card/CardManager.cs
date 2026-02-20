using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = System.Random;
using Napadol.Tools;

namespace CardProject
{
    public class CardManager : ActionListManager
    {
        public Card cardPrefab;

        public System.Action trickEnded;
        public System.Action cardSetUpDone;
        public System.Action player2Played;
        public System.Action player3Played;
        public System.Action player4Played;
        
        #region CardVariables
        private List<Card> cards = new List<Card>();
        private DeckData freeDeck = new DeckData(Vector3.zero, DeckData.DeckHoldType.None, false, 0);
        public DeckData GetFreeDeck()
        {
            return freeDeck;
        }
        public DeckData playDeck{ get; private set; }
        public DeckData drawDeck{ get; private set; }
        public DeckData discardDeck{ get; private set; }
        public DeckData player1Hand{ get; private set; }
        public DeckData player2Hand{ get; private set; }
        public DeckData player3Hand{ get; private set; }
        public DeckData player4Hand{ get; private set; }
        private Card currentHoverCard;
        private Card currentPlayedCard;
        
        [SerializeField] private GameObject hand1Position;
        [SerializeField] private GameObject hand2Position;
        [SerializeField] private GameObject hand3Position;
        [SerializeField] private GameObject hand4Position;
        [SerializeField] private GameObject drawDeckPosition;
        [SerializeField] private GameObject playDeckPosition;
        [SerializeField] private GameObject discardDeckPosition;
        #endregion
        
        private float DEFAULTDELAY = 0.01f;
        private float DEFAULTSTACKEDPOSOFFSET = 0.05f;
        private float DEFAULTSTACKEDZOFFSET = 0.11f;
        private float DEFAULTFLIPDURATION = 0.5f;
        private float DEFAULTMOVEDURATION = 0.25f;

        private bool isResetting = false;
        private bool isStarted = false;
        private bool cardIsSelected = false;
        
        public void SetResettingBool(bool b)
        {
            isResetting = b;
        }
        
        private void Start()
        {
            playDeck = new DeckData(playDeckPosition.transform.position, DeckData.DeckHoldType.Stacked, false, 0);
            drawDeck = new DeckData(drawDeckPosition.transform.position, DeckData.DeckHoldType.Stacked, true, 0);
            discardDeck = new DeckData(discardDeckPosition.transform.position, DeckData.DeckHoldType.UnorganizedStacked, false, 67);
            player1Hand = new DeckData(hand1Position.transform.position, DeckData.DeckHoldType.Spread, false, 0);
            player2Hand = new DeckData(hand2Position.transform.position, DeckData.DeckHoldType.Spread, true, -90);
            player3Hand = new DeckData(hand3Position.transform.position, DeckData.DeckHoldType.Spread, true, 180);
            player4Hand = new DeckData(hand4Position.transform.position, DeckData.DeckHoldType.Spread, true, 90);

            //trickEnded += HandleHandNumber;
            
            //AllCardinit
            for (int i = 0; i < 52; i++)
            {
                Card currentCard = SpawnCard(i, RandomPosition());
                cards.Add(currentCard);
                freeDeck.cards.Add(currentCard);
            }

            MoveCardsIntoDeck(freeDeck, drawDeck, 52,DEFAULTDELAY * 5, true,true, 0.0f);
            actionList.AddAction(new CallBackAction(() => ShuffleThisDeck(drawDeck), nameof(ShuffleThisDeck), true, 0.0f));
            actionList.AddAction(new CallBackAction(() => DealCardsToAllPlayer(GameManager.instance.handSize), nameof(DealCardsToAllPlayer), true, 0.0f));
            actionList.AddAction(new CallBackAction(() => CallACallBack(() => InvokeCardSetUpDone(), nameof(InvokeCardSetUpDone), true, 0.0f
            ), nameof(CallACallBack), true, 0.0f));
            actionList.AddAction(new WaitAction(0.5f));
            actionList.AddAction(new CallBackAction(() => InvokeTrickStarted(), nameof(InvokeTrickStarted), true, 0.0f));
        }

        private void InvokeTrickStarted()
        {
            trickEnded?.Invoke();
            isStarted = true;
        }

        protected override void DerivedUpdate()
        {
            CardUpdateOnHand(player1Hand);
            //actionList.RunActions(Time.deltaTime * timeMultiplier);
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
            card1.name = "Card" + i.ToString();
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
            MoveAction moveAction = new MoveAction(cards[i].gameObject, false, i * 0.5f, 20.0f, RandomPosition(), Easing.EaseOutBack);
            SpinAction rotateAction = new SpinAction(cards[i].gameObject,false, i * 0.5f, 500.0f,float.MaxValue, isRight, Easing.EaseOutExpo);
            Action.SynchronizeDurationFirstToSecond(moveAction, rotateAction);
            list.Add(moveAction);
            list.Add(rotateAction);
                
            list.Add(new FlipAction(cards[i].gameObject,false, 0.0f, 0.2f, Easing.EaseLinear));
                
            Vector2 finalScale = new Vector2(1.0f, 1.0f);
            if (cards[i].gameObject.transform.localScale.x <= 1.05f)
            {
                finalScale = new Vector2(3.0f, 3.0f);
            }

            ScaleAction scaleAction = new ScaleAction(cards[i].gameObject, false, i * 0.5f, finalScale, 3.0f, Easing.EaseOutExpo);
            list.Add(scaleAction);
            Action.SynchronizeDurationFirstToSecond(moveAction, scaleAction);   
        }
        #endregion

        public void ResetTheGame()
        {
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(discardDeck, drawDeck, discardDeck.cards.Count, DEFAULTDELAY * 5, true, false, 0.0f), 
                nameof(MoveCardsIntoDeck), false, 0.0f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(playDeck, drawDeck, playDeck.cards.Count, DEFAULTDELAY * 5, true, false, 0.1f), 
                nameof(MoveCardsIntoDeck), false, 0.0f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(player1Hand, drawDeck, player1Hand.cards.Count, DEFAULTDELAY * 5, true, false, 0.2f), 
                nameof(MoveCardsIntoDeck), false, 0.0f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(player2Hand, drawDeck, player2Hand.cards.Count, DEFAULTDELAY * 5, true, false, 0.3f), 
                nameof(MoveCardsIntoDeck), false, 0.0f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(player3Hand, drawDeck, player3Hand.cards.Count, DEFAULTDELAY * 5, true, false, 0.4f), 
                nameof(MoveCardsIntoDeck), false, 0.0f));
            actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(player4Hand, drawDeck, player4Hand.cards.Count, DEFAULTDELAY * 5, true, true, 0.5f), 
                nameof(MoveCardsIntoDeck), true, 0.0f));
            /*actionList.AddAction(new CallBackAction(() => ShuffleThisDeck(drawDeck), nameof(ShuffleThisDeck), false, 0.7f));
            actionList.AddAction(new CallBackAction(() => DealCardsToAllPlayer(GameManager.instance.handSize), nameof(DealCardsToAllPlayer), false, 2.0f));*/
            actionList.AddAction(new CallBackAction(() => CallACallBack(()=>ShuffleThisDeck(drawDeck), nameof(ShuffleThisDeck), false, 1.5f), nameof(CallACallBack), false, 0.0f));
            actionList.AddAction(new CallBackAction(() => CallACallBack(()=>DealCardsToAllPlayer(GameManager.instance.handSize), nameof(DealCardsToAllPlayer), false, 3.0f),
                nameof(CallACallBack), false, 0.0f));
        }
        
        public void MoveCardsIntoDeck(DeckData moveFromDeck, DeckData moveToDeck, int amountOfCards, float delayEachCard, bool willRotate, bool nestedBlock, float nestedDelay)
        {
            if (amountOfCards <= 0)
            {
                return;
            }
            
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
                        moveAction = new MoveAction(DEFAULTMOVEDURATION, currentCard.gameObject, false, delayEachCard * i, PosMoveCardStackedStyle(moveToDeck.cards.Count - 1, moveToDeck.currentPosition, originalNotChosenDeckSize), Easing.EaseOutBack);
                        newNestList.Add(moveAction);
                        break;
                    case DeckData.DeckHoldType.Spread:
                        MoveCardIntoSpreadDeck(ref newNestList,moveToDeck, delayEachCard, moveToDeck.cards.Count - 1);
                        break;
                    case DeckData.DeckHoldType.UnorganizedStacked:
                        moveAction = new MoveAction(DEFAULTMOVEDURATION, currentCard.gameObject, false, delayEachCard * i, 
                            PosMoveCardStackedStyle(moveToDeck.cards.Count - 1, moveToDeck.currentPosition, originalNotChosenDeckSize) + (Vector3.right * UnityEngine.Random.Range(-1.0f, 1.0f)) + (Vector3.up * UnityEngine.Random.Range(-1.0f, 1.0f)), Easing.EaseOutBack);
                        newNestList.Add(moveAction);
                        break;
                    case DeckData.DeckHoldType.None:
                        break;
                }
                
                if (moveToDeck.isFlipDown != moveFromDeck.isFlipDown)
                {
                    newNestList.Add(new FlipAction(currentCard.gameObject, false, delayEachCard * i, DEFAULTFLIPDURATION, Easing.EaseLinear));
                }

                if (willRotate)
                {
                    switch (moveToDeck.currentHoldType)
                    {
                        case DeckData.DeckHoldType.Stacked:
                            newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * i, 1.0f, moveToDeck.deckAngle, 1, true, Easing.EaseOutExpo));
                            break;
                        case DeckData.DeckHoldType.Spread:
                            
                            newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * i, 1.0f, -1 * moveToDeck.SpreadOrganicRotateCalculation(i, moveToDeck.deckAngle, amountOfCards), 1, true, Easing.EaseOutExpo));
                            
                            break;
                        case DeckData.DeckHoldType.UnorganizedStacked:
                            newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * i, 1.0f, UnityEngine.Random.Range(0.0f, 360.0f), 1, true, Easing.EaseOutExpo));
                            break;
                        case DeckData.DeckHoldType.None:
                            break;
                    }
                }

                //remove it from the list
                moveFromDeck.Pop();
            }
            
            //Adjust the movefrom deck if needed AFTER REMOVE ALL MOVING CARDS from list
            /*newNestList.Add(new CallBackAction(() => AdjustThisDeck(ref newNestList, moveFromDeck, delayEachCard), true, 0.0f, 0.2f));
            newNestList.Add(new CallBackAction(() => AdjustThisDeck(ref newNestList, moveToDeck, delayEachCard), true, 0.0f, 0.2f));*/
            AdjustThisDeck(ref newNestList, moveFromDeck, delayEachCard);
            //AdjustThisDeck(ref newNestList, moveToDeck, delayEachCard);
            
            NestedAction nestedList = new NestedAction(newNestList, nestedBlock, nestedDelay);
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
                    moveAction = new MoveAction(DEFAULTMOVEDURATION, currentCard.gameObject, false, delayEachCard * theIndex, PosMoveCardStackedStyle(moveToDeck.cards.Count - 1, moveToDeck.currentPosition, originalNotChosenDeckSize), Easing.EaseOutBack);
                    newNestList.Add(moveAction);
                    break;
                case DeckData.DeckHoldType.Spread:
                    MoveCardIntoSpreadDeck(ref newNestList,moveToDeck, delayEachCard, moveToDeck.cards.Count - 1);
                    break;
                case DeckData.DeckHoldType.UnorganizedStacked:
                    moveAction = new MoveAction(DEFAULTMOVEDURATION, currentCard.gameObject, false, delayEachCard * theIndex, 
                        PosMoveCardStackedStyle(moveToDeck.cards.Count - 1, moveToDeck.currentPosition, originalNotChosenDeckSize) + (Vector3.right * UnityEngine.Random.Range(-1.0f, 1.0f)) + (Vector3.up * UnityEngine.Random.Range(-1.0f, 1.0f)), Easing.EaseOutBack);
                    newNestList.Add(moveAction);
                    break;
                case DeckData.DeckHoldType.None:
                    break;
            }
            //If it needs to flip or not
            if (moveToDeck.isFlipDown != moveFromDeck.isFlipDown) // Deck check, not good, card check good
            {
                newNestList.Add(new FlipAction(currentCard.gameObject, false, delayEachCard * theIndex, DEFAULTFLIPDURATION, Easing.EaseLinear));
            }

            if (willRotate)
            {
                switch (moveToDeck.currentHoldType)
                {
                    case DeckData.DeckHoldType.Stacked:
                        newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * theIndex, 1.0f,
                            moveToDeck.deckAngle, 1, true, Easing.EaseOutExpo));
                        break;
                    case DeckData.DeckHoldType.Spread:
                        /*newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * theIndex, 1.0f,
                            moveToDeck.SpreadOrganicRotateCalculation(theIndex, moveToDeck.deckAngle), 1, true));*/
                        break;
                    case DeckData.DeckHoldType.UnorganizedStacked:
                        newNestList.Add(new RotateAction(currentCard.gameObject, false, delayEachCard * theIndex, 1.0f,
                            UnityEngine.Random.Range(0.0f, 360.0f), 1, true, Easing.EaseOutExpo));
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
                    //Debug.Log(moveFromDeck.cards.Count);
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
            if (index >= moveFromDeck.cards.Count)
            {
                return;
            }
            SelectCardsIntoDeck(moveFromDeck,moveFromDeck.cards[index], moveToDeck, delayEachCard, willRotate);
        }

        public void ShuffleThisDeck(DeckData chosendeck)
        {
            //actionList.AddAction(new WaitAction(0.0f));
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
                newNestList.Add(new RotateAction(chosendeck.cards[n].gameObject, false, i * DEFAULTDELAY * 1.5f, 0.5f, 0, 1, false, Easing.EaseOutExpo));
                newNestList.Add(new RotateAction(chosendeck.cards[n].gameObject, false, i * DEFAULTDELAY * 1.5f, 0.5f, 0f, 1, false, Easing.EaseOutExpo));
                newNestList.Add(new MoveAction(0.1f, chosendeck.cards[n].gameObject, false, i * DEFAULTDELAY * 1.5f, posn, Easing.EaseOutBack)); // SWAP INSTANTLY
                newNestList.Add(new RotateAction(chosendeck.cards[k].gameObject, false, i * DEFAULTDELAY * 1.5f, 0.5f, 0, 1, true, Easing.EaseOutExpo));
                newNestList.Add(new MoveAction(0.1f, chosendeck.cards[k].gameObject, false, i * DEFAULTDELAY * 1.5f, posk, Easing.EaseOutBack));
                i++;
            }
            
            List<Action> nNestList = new List<Action>();
            for (int j = 0; j < chosendeck.cards.Count; j++)
            {
                Vector3 correctPos = PosMoveCardStackedStyle(j, chosendeck.currentPosition, chosendeck.cards.Count);
                newNestList.Add(new MoveAction(0.1f, chosendeck.cards[j].gameObject, false, i * DEFAULTDELAY * 1.5f, correctPos, Easing.EaseOutBack));
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
                actions.Add(new MoveAction(DEFAULTMOVEDURATION, chosendeck.cards[i].gameObject, false, delayEachCard * index, chosendeck.SpreadCardPosXCalculation(i, chosendeck.deckAngle), Easing.EaseOutBack));
            }
            chosendeck.cards[^1].deckZ = chosendeck.SpreadCardPosXCalculation(chosendeck.cards.Count - 1, chosendeck.deckAngle).z;
        }

        private void AdjustThisDeck(ref List<Action> actions, DeckData chosendeck, float delayEachCard)
        {
            switch (chosendeck.currentHoldType)
            {
                case DeckData.DeckHoldType.Stacked:
                    break;
                case DeckData.DeckHoldType.Spread:
                    AdjustSpreadDeck(ref actions, chosendeck, delayEachCard);
                    break;
                case DeckData.DeckHoldType.UnorganizedStacked:
                    break;
                case DeckData.DeckHoldType.None:
                        
                    break;
            }
        }

        public void AdjustDrawDeck()
        {
            for (int j = 0; j < drawDeck.cards.Count; j++)
            {
                Vector3 correctPos = PosMoveCardStackedStyle(j, drawDeck.currentPosition, drawDeck.cards.Count);
                actionList.AddAction(new MoveAction(0.1f, drawDeck.cards[j].gameObject, false, j * DEFAULTDELAY * 1.5f, correctPos, Easing.EaseInCirc));
            }
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
                MoveAction mv = new MoveAction(DEFAULTMOVEDURATION, chosendeck.cards[i].gameObject, false,
                    delayEachCard * i,
                    chosendeck.SpreadCardPosXCalculation(i, chosendeck.deckAngle), Easing.EaseOutBack);
                
                actions.Add(mv);
                RotateAction rt = new RotateAction(chosendeck.cards[i].gameObject, false, delayEachCard * i,
                    float.MaxValue, chosendeck.SpreadOrganicRotateCalculation(i, chosendeck.deckAngle, chosendeck.cards.Count), Easing.EaseOutExpo);
                rt.SynchronizeDurationFromThisAction(mv);
                actions.Add(rt);
            }
        }

        private void CardUpdateOnHand(DeckData data)
        {
            if (isResetting)
            {
                return;
            }
            
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

            if (currentPlayedCard != null)
            {
                if (!actionList.IsEmpty())
                {
                    return;
                }
                else
                {
                    currentPlayedCard = null;
                }
            }
            
            if (currentHoverCard != null && (Input.GetMouseButtonDown(0) || cardIsSelected))
            {
                currentPlayedCard = currentHoverCard;
                cardIsSelected = false;
                
                // Do something when clicking on a card
                actionList.AddAction(new RotateAction(currentHoverCard.gameObject, false, 0.0f, 0.5f, 0.0f, Easing.EaseOutExpo));
                SelectCardsIntoDeck(player1Hand, currentHoverCard, playDeck, 0.0f, false);
                actionList.AddAction(new ScaleAction(currentHoverCard.gameObject, false, 0.0f, new Vector2(1.0f, 1.0f), 0.15f, Easing.EaseOutExpo));
                //actionList.AddAction(new WaitAction(0.3f));

                
                //CHANGE THIS TO DEAL FOR EACH PLAYER 1 CARD AT A TIME
                actionList.AddAction(new CallBackAction(() => CallACallBack(() => InvokePlayer2Played(), nameof(InvokePlayer2Played), false, 0.0f), nameof(CallACallBack), false, 0.0f));
                actionList.AddAction(new CallBackAction(() => MoveAnIndexOfCardIntoDeck(player2Hand, UnityEngine.Random.Range(0, player2Hand.cards.Count), playDeck, 0.0f, false), nameof(MoveAnIndexOfCardIntoDeck), true, 0.0f));
                actionList.AddAction(new CallBackAction(() => CallACallBack(() => InvokePlayer3Played(), nameof(InvokePlayer3Played), false, 0.0f), nameof(CallACallBack), false, 0.0f));
                actionList.AddAction(new CallBackAction(() => MoveAnIndexOfCardIntoDeck(player3Hand, UnityEngine.Random.Range(0, player2Hand.cards.Count), playDeck, 0.0f, false), nameof(MoveAnIndexOfCardIntoDeck), true, 0.0f));
                actionList.AddAction(new CallBackAction(() => CallACallBack(() => InvokePlayer4Played(), nameof(InvokePlayer4Played), false, 0.0f), nameof(CallACallBack), false, 0.0f));
                actionList.AddAction(new CallBackAction(() => MoveAnIndexOfCardIntoDeck(player4Hand, UnityEngine.Random.Range(0, player2Hand.cards.Count), playDeck, 0.0f, false), nameof(MoveAnIndexOfCardIntoDeck), true, 0.0f));
                //actionList.AddAction(new CallBackAction(() => CallACallBack(() => InvokeTrickHasEnd(), nameof(InvokeTrickHasEnd), false, 0.0f), nameof(CallACallBack), false, 0.0f));
                actionList.AddAction(new CallBackAction(() => InvokeTrickHasEnd(), nameof(CallACallBack), false, 0.0f));
                actionList.AddAction(new CallBackAction(() => MoveCardsIntoDeck(playDeck, discardDeck, playDeck.cards.Count, 0.2f, true, true, 0.2f), nameof(MoveCardsIntoDeck), true, 0.5f));
            }
            
            // If we're hovering over a different card than before
            if (newHoverCard != currentHoverCard)
            {
                // Scale down the previous card if there was one
                if (currentHoverCard != null)
                {
                    actionList.AddAction(new ScaleAction(currentHoverCard.gameObject, false, 0.0f, new Vector2(1.0f, 1.0f), 0.25f, Easing.EaseOutExpo));
                }
        
                // Scale up the new card if there is one
                if (newHoverCard != null)
                {
                    actionList.AddAction(new ScaleAction(newHoverCard.gameObject, false, 0.0f, new Vector2(1.2f, 1.2f), 0.25f, Easing.EaseOutExpo));
                }
        
                // Update current hover card
                currentHoverCard = newHoverCard;
            }
        }

        public bool PlayACard()
        {
            if (!isStarted)
            {
                return false;
            }
            
            if (currentPlayedCard == null)
            {
                currentHoverCard = player1Hand.cards[UnityEngine.Random.Range(0, player1Hand.cards.Count)];
                cardIsSelected = true;
                return true;
            }
            
            return false;
        }
        
        #region Invokers

        private void InvokePlayer2Played()
        {
            player2Played?.Invoke();
        }
        
        private void InvokePlayer3Played()
        {
            player3Played?.Invoke();
        }
        
        private void InvokePlayer4Played()
        {
            player4Played?.Invoke();
        }
        
        private void InvokeTrickHasEnd()
        {
            trickEnded?.Invoke();
        }

        private void InvokeCardSetUpDone()
        {
            cardSetUpDone?.Invoke();
        }

        #endregion
        
        private void CallACallBack(System.Action actionToCallBack, string nameOfFunc, bool blocking, float delay)
        {
            actionList.AddAction(new CallBackAction(actionToCallBack, nameOfFunc, blocking, delay));
        }
        
        public void DealCardsToAllPlayer(int amount)
        {
            if (amount == 0)
            {
                return;
            }
            
            switch (GameManager.instance.handNumber)
            {
                case 2:
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(drawDeck, player1Hand, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    if (player3Hand.cards.Count <= 0) MoveCardsIntoDeck(drawDeck, player3Hand, amount, DEFAULTDELAY, true, false, 0.07f * 2);
                    break;
                case 3:
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(drawDeck, player1Hand, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    if (player2Hand.cards.Count <= 0) MoveCardsIntoDeck(drawDeck, player2Hand, amount, DEFAULTDELAY, true, false, 0.07f * 1);
                    if (player3Hand.cards.Count <= 0) MoveCardsIntoDeck(drawDeck, player3Hand, amount, DEFAULTDELAY, true, false, 0.07f * 2);
                    break;
                case 4:
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(drawDeck, player1Hand, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    if (player2Hand.cards.Count <= 0) MoveCardsIntoDeck(drawDeck, player2Hand, amount, DEFAULTDELAY, true, false, 0.07f * 1);
                    if (player3Hand.cards.Count <= 0) MoveCardsIntoDeck(drawDeck, player3Hand, amount, DEFAULTDELAY, true, false, 0.07f * 2);
                    if (player4Hand.cards.Count <= 0) MoveCardsIntoDeck(drawDeck, player4Hand, amount, DEFAULTDELAY, true, false, 0.07f * 3);
                    break;
            }
            actionList.AddAction(new WaitAction(0.5f));
        }
        
        public void FillCardsToAllPlayer(int amount)
        {
            if (amount == 0)
            {
                return;
            }
            
            switch (GameManager.instance.handNumber)
            {
                case 2:
                    MoveCardsIntoDeck(drawDeck, player1Hand, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    MoveCardsIntoDeck(drawDeck, player3Hand, amount, DEFAULTDELAY, true, false, 0.07f * 2);
                    break;
                case 3:
                    MoveCardsIntoDeck(drawDeck, player1Hand, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    MoveCardsIntoDeck(drawDeck, player2Hand, amount, DEFAULTDELAY, true, false, 0.07f * 1);
                    MoveCardsIntoDeck(drawDeck, player3Hand, amount, DEFAULTDELAY, true, false, 0.07f * 2);
                    break;
                case 4:
                    MoveCardsIntoDeck(drawDeck, player1Hand, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    MoveCardsIntoDeck(drawDeck, player2Hand, amount, DEFAULTDELAY, true, false, 0.07f * 1);
                    MoveCardsIntoDeck(drawDeck, player3Hand, amount, DEFAULTDELAY, true, false, 0.07f * 2);
                    MoveCardsIntoDeck(drawDeck, player4Hand, amount, DEFAULTDELAY, true, false, 0.07f * 3);
                    break;
            }
            actionList.AddAction(new WaitAction(0.5f));
        }
        
        public void RemoveCardsFromAllPlayer(int amount)
        {
            if (amount == 0)
            {
                return;
            }
            
            switch (GameManager.instance.handNumber)
            {
                case 2:
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(player1Hand, discardDeck, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(player3Hand, discardDeck, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    break;
                case 3:
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(player1Hand, discardDeck, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(player2Hand, discardDeck, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(player3Hand, discardDeck, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    break;
                case 4:
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(player1Hand, discardDeck, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(player2Hand, discardDeck, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(player3Hand, discardDeck, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    if (player1Hand.cards.Count <= 0) MoveCardsIntoDeck(player4Hand, discardDeck, amount, DEFAULTDELAY, true, false, 0.07f * 0);
                    break;
            }
            actionList.AddAction(new WaitAction(0.5f));
        }

        public void RefillCardInAllHandsToHandSize()
        {
            if (isStarted)
            {
                int c = GameManager.instance.handSize - player1Hand.cards.Count;
                
                if (c < 0)
                {
                    RemoveCardsFromAllPlayer(-c);
                }
                else if(c > 0)
                {
                    FillCardsToAllPlayer(c);
                }
            }
        }
        
        private void ShuffleDrawDeckViaInput()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                actionList.ClearActions();
                ShuffleThisDeck(drawDeck);
            }
        }

        public bool AllHandsHaveNoCards()
        {
            if (player1Hand.cards.Count <= 0 && player2Hand.cards.Count <= 0 && player3Hand.cards.Count <= 0 && player4Hand.cards.Count <= 0)
            {
                return true;
            }
            return false;
        }

        public void RefilDrawDeckWithDiscardPileAfterHandOut()
        {
            RefilDrawDeckWithDiscardPile();
            actionList.AddAction(new CallBackAction(() => DealCardsToAllPlayer(GameManager.instance.handSize), nameof(DealCardsToAllPlayer), true, 0.0f));
        }
        
        public void RefilDrawDeckWithDiscardPile()
        { 
            MoveCardsIntoDeck(discardDeck, drawDeck, discardDeck.cards.Count, DEFAULTDELAY, true, true, 0.0f);
            actionList.AddAction(new CallBackAction(() => ShuffleThisDeck(drawDeck), nameof(ShuffleThisDeck), true, 0.0f));
        }

        public void HandleHandNumber()
        {
            /*if (currentPlayedCard != null)
            {
                return;
            }*/
            
            switch (GameManager.instance.handNumber)
            {
                case 2:
                    EmptyHand(player2Hand);
                    EmptyHand(player4Hand);
                    break;
                case 3:
                    RefillHand(player2Hand);
                    EmptyHand(player4Hand);
                    break;
                case 4:
                    RefillHand(player2Hand);
                    RefillHand(player4Hand);
                    break;
            }
        }

        private void RefillHand(DeckData playerHand)
        {
            if (playerHand.cards.Count > 0)
            {
                return;
            }

            if (!isStarted) // Don't allow changing handNumber until game start because its buggy
            {
                return;
            }
            
            int final = 0;
            if (GameManager.instance.currentTrick == 0)
            {
                final = GameManager.instance.handSize - (GameManager.instance.currentTrick);
            }
            else
            {
                final = GameManager.instance.handSize - (GameManager.instance.currentTrick - 1);
            }
            
            if (final >= drawDeck.cards.Count)
            {
                RefilDrawDeckWithDiscardPile();
            }
            
            MoveCardsIntoDeck(drawDeck, playerHand, final, DEFAULTDELAY * 5, true, false, 0.0f);

            /*void OnTrickEnd()
            {
                if (playerHand.cards.Count > 0)
                {
                    trickEnded -= OnTrickEnd;
                }
                
                //func
                MoveCardsIntoDeck(drawDeck, playerHand, GameManager.instance.handSize - (GameManager.instance.currentTrick - 1), DEFAULTDELAY * 5, true, false, 0.0f);
                
                trickEnded -= OnTrickEnd;
            }
            
            trickEnded += OnTrickEnd;*/
        }

        private void EmptyHand(DeckData playerHand)
        {
            if (playerHand.cards.Count <= 0)
            {
                return;
            }
            MoveCardsIntoDeck(playerHand, discardDeck, playerHand.cards.Count, 0.05f, true, false, 0.0f);
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        
        [SerializeField] private CardManager cardManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private GameObject debugTexts;
        
        private ActionList cardActionList;
        private ActionList uiActionList;
        private bool cardIsPaused = false;

        private int p1Score = 0;
        private int p2Score = 0;
        private int p3Score = 0;
        private int p4Score = 0;
        private int winner = -1;

        public enum PlaySpeedState
        {
            Slow,
            Normal, 
            Fast,
            Crazy
        }
        public PlaySpeedState currentPlaySpeed = PlaySpeedState.Normal;
        private float currentPlaySpeedMultiplier = 1.0f;
        public float GetCurrentPlaySpeed()
        {
            return currentPlaySpeedMultiplier;
        }

        private float slowPlaySpeed = 0.5f;
        private float normalPlaySpeed = 1.0f;
        private float fastPlaySpeed = 2.0f;
        private float crazyPlaySpeed = 5.0f;
        
        public int currentPlayerNumber = 4;
        
        private int scoreToWin = 3; // first to this score wins
        
        [HideInInspector]
        public int currentTrick = 0;

        [HideInInspector]
        public int handSize = 7;
        private const int MAXHANDSIZE = 10;

        [HideInInspector]
        public int handNumber = 4;

        private bool runOnce = false;

        [HideInInspector]
        public int escCount;
        
        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            ResetGameData();
            cardManager.cardSetUpDone += uiManager.FadeInHUD;
            cardManager.player2Played += HandleUpdatingHUDPlayer2;
            cardManager.player3Played += HandleUpdatingHUDPlayer3;
            cardManager.player4Played += HandleUpdatingHUDPlayer4;
            cardManager.trickEnded += TrickEndsHandling;
            cardActionList = cardManager.actionList;
        }

        
        
        void Update()
        {
            if (!runOnce){ TogglePausing(); uiManager.HandNumberButton._onUp += ChangeHandNumber; runOnce = true; }
            
            DebugInput();
            HandleUpdatingHUD();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePausing();
            }
        }
        private void DebugInput()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                debugTexts.SetActive(!debugTexts.activeInHierarchy);
            }
        }

        public void TogglePausing()
        {
            cardIsPaused = !cardIsPaused;
            if (cardIsPaused)
            {
                cardManager.GetComponent<ActionListManager>().LerpTimeMultiplier(0f);
                uiManager.TogglePauseMenu();
            }
            else
            {
                cardManager.GetComponent<ActionListManager>().LerpTimeMultiplier(currentPlaySpeedMultiplier);
                uiManager.TogglePauseMenu();
                cardManager.HandleHandNumber();
                escCount++;
                TelemetryGenerator.instance.Log();
            }
        }

        private void TrickEndsHandling()
        {
            if (currentTrick > 0) // don't run when started
            {
                //Random Winner, give score to that person
                switch (UnityEngine.Random.Range(0, handNumber))
                {
                    case 0:
                        p1Score++;
                        uiManager.PlayerGetScore(uiManager.player1HUD, p1Score);
                        break;
                    case 1:
                        p3Score++;
                        uiManager.PlayerGetScore(uiManager.player3HUD, p3Score);
                        break;
                    case 2:
                        p2Score++;
                        uiManager.PlayerGetScore(uiManager.player2HUD, p2Score);
                        break;
                    case 3:
                        p4Score++;
                        uiManager.PlayerGetScore(uiManager.player4HUD, p4Score);
                        break;
                }
            }
            
            if (WinCheck())
            {
                //Game Ends
                //THIS PLAYER WINS!!
                uiManager.TriggerTrickCountWINAnimation(winner);
                //RESHUFFLE START THE GAME AGAIN
                uiManager.FadeOutHUD(true);
                cardManager.ResetTheGame();
                cardActionList.AddAction(new CallBackAction(() => cardActionList.CallACallBack(() => uiManager.ResetHUD(), nameof(uiManager.ResetHUD), false, 0.0f), nameof(cardActionList.CallACallBack), false, 0.0f));
                cardActionList.AddAction(new CallBackAction(() => cardActionList.CallACallBack(() => uiManager.FadeInHUD(),nameof(uiManager.FadeInHUD), false, 4.0f), nameof(cardActionList.CallACallBack), false, 0.0f));
                //RESET DATA
                ResetGameData();
                
            }
            else
            {
                currentTrick++;
                cardActionList.AddAction(new CallBackAction(() => uiManager.TriggerTrickCountAnimation(currentTrick), nameof(uiManager.TriggerTrickCountAnimation), true, 0.0f));
                
                if (cardManager.AllHandsHaveNoCards())
                {
                    cardActionList.AddAction(new CallBackAction(() => cardManager.RefilDrawDeckWithDiscardPileAfterHandOut(), nameof(cardManager.RefilDrawDeckWithDiscardPileAfterHandOut), true, 0.0f));
                }
            }

            //cardManager.AdjustDrawDeck();
            //cardActionList.AddAction(new CallBackAction(() => cardActionList.CallACallBack(() => cardManager.AdjustDrawDeck(), nameof(cardManager.AdjustDrawDeck), true, 0.0f), nameof(cardActionList.CallACallBack), true, 0.0f));

        }

        private void ResetGameData()
        {
            currentTrick = 0;
            p1Score = 0;
            p2Score = 0;
            p3Score = 0;
            p4Score = 0;
            winner = -1;
        }

        private bool WinCheck()
        {
            bool check = p1Score >= scoreToWin || p2Score >= scoreToWin || p3Score >= scoreToWin || p4Score >= scoreToWin;
            if (check){
                if (p1Score >= scoreToWin)
                {
                    winner = 0;
                }
                else if (p2Score >= scoreToWin)
                {
                    winner = 1;
                }
                else if (p3Score >= scoreToWin)
                {
                    winner = 2;
                }
                else if (p4Score >= scoreToWin)
                {
                    winner = 3;
                }
            }
            return check;
        }
        
        private void HandleUpdatingHUD()
        {
            uiManager.player1HUD.EditCardLeftNumber(cardManager.player1Hand.cards.Count);
        }

        public void ChangePlaySpeed()
        {
            if (currentPlaySpeed == PlaySpeedState.Crazy)
            {
                currentPlaySpeed = PlaySpeedState.Slow;
            }
            else
            {
                currentPlaySpeed++;
            }
            
            switch (currentPlaySpeed)
            {
                case PlaySpeedState.Slow:
                    currentPlaySpeedMultiplier = slowPlaySpeed;
                    break;
                case PlaySpeedState.Normal:
                    currentPlaySpeedMultiplier = normalPlaySpeed;
                    break;
                case PlaySpeedState.Fast:
                    currentPlaySpeedMultiplier = fastPlaySpeed;
                    break;
                case PlaySpeedState.Crazy:
                    currentPlaySpeedMultiplier = crazyPlaySpeed;
                    break;
            }
        }

        public void ChangeHandNumber()
        {
            switch (handNumber)
            {
                case 2:
                    handNumber = 3;
                    break;
                case 3:
                    handNumber = 4;
                    break;
                case 4:
                    handNumber = 2;
                    break;
            }
            uiManager.EditHandNumberText(handNumber);
        }
        
        public void ChangeHandSize()
        {
            // 1-MAXHANDSIZE
            handSize = (handSize % MAXHANDSIZE) + 1;
        }

        public int GetPlayerScore(int playerNumber) //1234
        {
            switch (playerNumber)
            {
                case 1:
                    return p1Score;
                case 2:
                    return p2Score;
                case 3:
                    return p3Score;
                case 4:
                    return p4Score;
            }

            return -1;
        }
        
        #region EvilCode

        private void HandleUpdatingHUDPlayer2()
        {
            uiManager.player2HUD.EditCardLeftNumber(cardManager.player2Hand.cards.Count);
        }

        private void HandleUpdatingHUDPlayer3()
        {
            uiManager.player3HUD.EditCardLeftNumber(cardManager.player3Hand.cards.Count);
        }

        private void HandleUpdatingHUDPlayer4()
        {
            uiManager.player4HUD.EditCardLeftNumber(cardManager.player4Hand.cards.Count);
        }

        #endregion
    }
    

}

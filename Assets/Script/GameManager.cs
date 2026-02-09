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

        private float slowPlaySpeed = 0.5f;
        private float normalPlaySpeed = 1.0f;
        private float fastPlaySpeed = 2.0f;
        private float crazyPlaySpeed = 5.0f;
        
        public int currentPlayerNumber = 4;
        
        public int scoreToWin = 3; // first to this score wins
        
        [HideInInspector]
        public int currentTrick = 0;
        
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
            cardManager.cardSetUpDone += uiManager.FadeInHUD;
            cardManager.player2Played += HandleUpdatingHUDPlayer2;
            cardManager.player3Played += HandleUpdatingHUDPlayer3;
            cardManager.player4Played += HandleUpdatingHUDPlayer4;
            cardManager.trickEnded += TrickEndsHandling;
        }

        
        
        void Update()
        {
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
            }
        }

        private void TrickEndsHandling()
        {
            //Random Winner, give score to that person
            switch (UnityEngine.Random.Range(0, 4))
            {
                case 0:
                    p1Score++;
                    uiManager.PlayerGetScore(uiManager.player1HUD, p1Score);
                    break;
                case 1:
                    p2Score++;
                    uiManager.PlayerGetScore(uiManager.player2HUD, p2Score);
                    break;
                case 2:
                    p3Score++;
                    uiManager.PlayerGetScore(uiManager.player3HUD, p3Score);
                    break;
                case 3:
                    p4Score++;
                    uiManager.PlayerGetScore(uiManager.player4HUD, p4Score);
                    break;
            }
            
            if (WinCheck())
            {
                //Game Ends
                //THIS PLAYER WINS!!
                uiManager.TriggerTrickCountWINAnimation(winner);
                //RESHUFFLE START THE GAME AGAIN
                cardManager.ResetTheGame();
            }
            else
            {
            currentTrick++;
            uiManager.TriggerTrickCountAnimation(currentTrick);
            }


        }

        private bool WinCheck()
        {
            bool check = p1Score >= scoreToWin || p2Score >= scoreToWin || p3Score >= scoreToWin || p4Score >= scoreToWin;
            if (check){
                if (p1Score >= scoreToWin)
                {
                    winner = 0;
                }
                else if (p2Score <= scoreToWin)
                {
                    winner = 1;
                }
                else if (p3Score <= scoreToWin)
                {
                    winner = 2;
                }
                else if (p4Score <= scoreToWin)
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

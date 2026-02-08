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
            HandlePausingCardManager();
            HandleUpdatingHUD();
        }
        private void DebugInput()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                debugTexts.SetActive(!debugTexts.activeInHierarchy);
            }
        }

        private void HandlePausingCardManager()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!cardIsPaused)
                {
                    cardManager.GetComponent<ActionListManager>().LerpTimeMultiplier(0f);
                    cardIsPaused = true;
                }
                else
                {
                    cardManager.GetComponent<ActionListManager>().LerpTimeMultiplier(1f);
                    cardIsPaused = false;
                }
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

            if (cardManager.AllHandsHaveNoCards())
            {
                //Game Ends
                //THIS PLAYER WINS!!
                //RESHUFFLE START THE GAME AGAIN
            }
        }

        private void HandleUpdatingHUD()
        {
            uiManager.player1HUD.EditCardLeftNumber(cardManager.player1Hand.cards.Count);
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

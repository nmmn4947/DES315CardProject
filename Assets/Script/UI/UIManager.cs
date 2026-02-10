using System;
using Napadol.Tools;
using TMPro;
using UnityEngine;

namespace CardProject
{
    public class UIManager : ActionListManager
    {
        [SerializeField] private CardManager cardManager;
        [SerializeField] private MenuButton menuButtonPrefab;
        [SerializeField] private PlayerSetHUD playerHUDTextPrefab;
        [SerializeField] private GameObject trickCountTextPrefab; // is a part of HUD
        [SerializeField] private GameObject menuRoot;
        [SerializeField] private GameObject hudRoot;
        [SerializeField] private GameObject hudPlayer1Object;
        [SerializeField] private GameObject hudPlayer2Object;
        [SerializeField] private GameObject hudPlayer3Object;
        [SerializeField] private GameObject hudPlayer4Object;
        
        #region HUDs
        [HideInInspector]
        public PlayerSetHUD player1HUD;
        [HideInInspector]
        public PlayerSetHUD player2HUD;
        [HideInInspector]
        public PlayerSetHUD player3HUD;
        [HideInInspector]
        public PlayerSetHUD player4HUD;
        
        private ActionList cardActionList;
        
        private TextMeshProUGUI trickCountText;
        private GameObject trickCountObj;
        private Vector3 offScreenRight;
        private Vector3 offScreenCenter;
        private Vector3 offScreenLeft;
        
        #endregion

        #region Menus
        
        [SerializeField] private GameObject pausedBG;
        [HideInInspector]
        public MenuButton ResumeButton;
        [HideInInspector]
        public MenuButton PlaySpeedButton;
        [HideInInspector]
        public MenuButton HandNumberButton;
        [HideInInspector]
        public MenuButton HandSizeButton;
        [HideInInspector]
        public MenuButton QuitButton;
        private Camera cam;
        private Vector3 outSideOfCanvas;
        private float offsetBetweenButtons = 70f;
        private bool menuIsOpen = false;
 
        #endregion

        private float DEFAULT_FADEDURATION = 0.5f;
        
        private void Start()
        {
            cardActionList = cardManager.actionList;
            
            #region MenuButtons
            cam = Camera.main;
            outSideOfCanvas = cam.ViewportToScreenPoint(new Vector3(0.5f, -0.25f, 0));
            
            //Generate MenuButtons
            ResumeButton = Instantiate(menuButtonPrefab, menuRoot.transform);
            ResumeButton.GetComponent<RectTransform>().position = outSideOfCanvas;
            ResumeButton._onUp += GameManager.instance.TogglePausing;
            ResumeButton.SetButtonText("Play");
            
            PlaySpeedButton = Instantiate(menuButtonPrefab, menuRoot.transform);
            PlaySpeedButton.GetComponent<RectTransform>().position = outSideOfCanvas;
            PlaySpeedButton._onUp += GameManager.instance.ChangePlaySpeed;
            PlaySpeedButton._onUp += ChangeSpeedText;
            PlaySpeedButton.SetButtonText("PlaySpeed : Normal");
            
            HandNumberButton = Instantiate(menuButtonPrefab, menuRoot.transform);
            HandNumberButton.GetComponent<RectTransform>().position = outSideOfCanvas;
            HandNumberButton.SetButtonText("HandNumber : 4");
            
            HandSizeButton = Instantiate(menuButtonPrefab, menuRoot.transform);
            HandSizeButton.GetComponent<RectTransform>().position = outSideOfCanvas;
            HandSizeButton.SetButtonText("HandSize");
            
            QuitButton = Instantiate(menuButtonPrefab, menuRoot.transform);
            QuitButton.GetComponent<RectTransform>().position = outSideOfCanvas;
            QuitButton._onUp += ExitApplication;
            QuitButton.SetButtonText("Quit");
            #endregion
            #region HUDs
            //Generate HUDS
            player1HUD = Instantiate(playerHUDTextPrefab, hudPlayer1Object.transform).GetComponent<PlayerSetHUD>();
            player1HUD.SetUpText("You", 0, 7);
            player2HUD = Instantiate(playerHUDTextPrefab, hudPlayer2Object.transform).GetComponent<PlayerSetHUD>();
            player2HUD.SetUpText("Dylan", 0, 7);
            player3HUD = Instantiate(playerHUDTextPrefab, hudPlayer3Object.transform).GetComponent<PlayerSetHUD>();
            player3HUD.SetUpText("Joseph", 0, 7);
            player4HUD = Instantiate(playerHUDTextPrefab, hudPlayer4Object.transform).GetComponent<PlayerSetHUD>();
            player4HUD.SetUpText("Andy", 0, 7);
            
            trickCountObj = Instantiate(trickCountTextPrefab, hudRoot.transform);
            trickCountText = trickCountObj.GetComponentInChildren<TextMeshProUGUI>();
            offScreenRight = cam.ViewportToScreenPoint(new Vector3(1.5f, 0.5f, 0.0f));
            offScreenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0.0f));
            offScreenLeft = cam.ViewportToScreenPoint(new Vector3(-0.5f, 0.5f, 0.0f));
            trickCountObj.GetComponent<RectTransform>().position = offScreenRight;
            
            #endregion
        }

        public void FadeOutHUD()
        {
            cardActionList.AddAction(new CVGroupFadeAction(hudRoot, true, 0.0f, DEFAULT_FADEDURATION, Easing.EaseLinear, 0.0f));
        }
        
        public void FadeInHUD()
        {
            cardActionList.AddAction(new CVGroupFadeAction(hudRoot, true, 0.0f, DEFAULT_FADEDURATION, Easing.EaseLinear, 1.0f));
        }

        public void PlayerGetScore(PlayerSetHUD playerHUD, int playerNewScore)
        {
            playerHUD.EditScoreNumber(playerNewScore);
            cardActionList.AddAction(new ScaleAction(playerHUD.gameObject, true, 0.0f, new Vector2(3, 3), 0.4f, Easing.EaseOutBounce));//Scale up
            cardActionList.AddAction(new WaitAction(0.5f));
            cardActionList.AddAction(new ScaleAction(playerHUD.gameObject, true, 0.0f, new Vector2(1, 1), 0.25f, Easing.EaseOutBack));//Scale down
        }

        public void TogglePauseMenu()
        {
            outSideOfCanvas = cam.ViewportToScreenPoint(new Vector3(0.5f, -0.25f, 0));
            menuIsOpen = !menuIsOpen;
            if (menuIsOpen)
            {
                actionList.ClearActions();
                Vector3 newPos = menuRoot.GetComponent<RectTransform>().localPosition;
                actionList.AddAction(new MoveRectTransformAction(newPos + new Vector3(0f, offsetBetweenButtons * 2f, 0f), ResumeButton.gameObject, false, 0.0f, 0.5f, Easing.EaseOutBounce));
                actionList.AddAction(new MoveRectTransformAction(newPos + new Vector3(0f, offsetBetweenButtons * 1f, 0f), PlaySpeedButton.gameObject, false, 0.05f, 0.5f, Easing.EaseOutBounce));
                actionList.AddAction(new MoveRectTransformAction(newPos + new Vector3(0f, offsetBetweenButtons * 0f, 0f), HandNumberButton.gameObject, false, 0.1f, 0.5f, Easing.EaseOutBounce));
                actionList.AddAction(new MoveRectTransformAction(newPos + new Vector3(0f, offsetBetweenButtons * -1f, 0f), HandSizeButton.gameObject, false, 0.15f, 0.5f, Easing.EaseOutBounce));
                actionList.AddAction(new MoveRectTransformAction(newPos + new Vector3(0f, offsetBetweenButtons * -2f, 0f), QuitButton.gameObject, false, 0.2f, 0.5f, Easing.EaseOutBounce));
                actionList.AddAction(new CVGroupFadeAction(pausedBG, false, 0.0f, 0.25f, Easing.EaseLinear, 1.0f));
            }
            else
            {
                actionList.ClearActions();
                Vector3 newPos = outSideOfCanvas;
                actionList.AddAction(new MoveRectTransformAction(true, newPos, ResumeButton.gameObject, false, 0.2f, 0.1f, Easing.EaseOutBack));
                actionList.AddAction(new MoveRectTransformAction(true, newPos, PlaySpeedButton.gameObject, false, 0.15f, 0.1f, Easing.EaseOutBack));
                actionList.AddAction(new MoveRectTransformAction(true, newPos, HandNumberButton.gameObject, false, 0.1f, 0.1f, Easing.EaseOutBack));
                actionList.AddAction(new MoveRectTransformAction(true, newPos, HandSizeButton.gameObject, false, 0.05f, 0.1f, Easing.EaseOutBack));
                actionList.AddAction(new MoveRectTransformAction(true, newPos, QuitButton.gameObject, false, 0.0f, 0.1f, Easing.EaseOutBack));
                actionList.AddAction(new CVGroupFadeAction(pausedBG, false, 0.0f, 0.25f, Easing.EaseLinear, 0.0f));
            }
        }

        private void ChangeSpeedText()
        {
            switch (GameManager.instance.currentPlaySpeed)
            {
                case GameManager.PlaySpeedState.Slow:
                    PlaySpeedButton.SetButtonText("PlaySpeed : Slow");
                    break;
                case GameManager.PlaySpeedState.Normal:
                    PlaySpeedButton.SetButtonText("PlaySpeed : Normal");
                    break;
                case GameManager.PlaySpeedState.Fast:
                    PlaySpeedButton.SetButtonText("PlaySpeed : Fast");
                    break;
                case GameManager.PlaySpeedState.Crazy:
                    PlaySpeedButton.SetButtonText("PlaySpeed : Crazy");
                    break;
            }

            
        }

        public void TriggerTrickCountAnimation(int count)
        {
            trickCountText.text = "Trick : " + count;
            
            cardActionList.AddAction(new MoveRectTransformAction(true, offScreenCenter, trickCountObj, true, 0.0f, 0.5f, Easing.EaseOutSine));
            cardActionList.AddAction(new WaitAction(0.5f));
            cardActionList.AddAction(new MoveRectTransformAction(true, offScreenLeft, trickCountObj, true, 0.0f, 0.5f, Easing.EaseInCirc));
            cardActionList.AddAction(new MoveRectTransformAction(true, offScreenRight, trickCountObj, true, 0.0f, 0.0f, Easing.EaseLinear));
        }
        
        public void TriggerTrickCountWINAnimation(int winnerPlayer)
        {
            string s = "";
            switch (winnerPlayer)
            {
                case 0:
                    s = "You";
                    break;
                case 1:
                    s = "Dylan";
                    break;
                case 2:
                    s = "Joseph";
                    break;
                case 3:
                    s = "Andy";
                    break;
            }
            
            trickCountText.text = s + " WIN!";
            
            cardActionList.AddAction(new MoveRectTransformAction(true, offScreenCenter, trickCountObj, true, 0.0f, 0.5f, Easing.EaseOutSine));
            cardActionList.AddAction(new WaitAction(0.5f));
            cardActionList.AddAction(new MoveRectTransformAction(true, offScreenLeft, trickCountObj, true, 0.0f, 0.5f, Easing.EaseInCirc));
            cardActionList.AddAction(new MoveRectTransformAction(true, offScreenRight, trickCountObj, true, 0.0f, 0.0f, Easing.EaseLinear));
        }

        private void ExitApplication()
        {
            Application.Quit();
        }
    }
}

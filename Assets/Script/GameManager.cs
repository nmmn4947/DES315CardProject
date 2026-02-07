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
            //cardManager.trickEnded += TrickEndedHandling;
        }

        void Update()
        {
            DebugInput();
            HandlePausingCardManager();
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
            }
        }
        
        private void HandleUpdatingHUD(){}
    }
    

}

using System;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace CardProject
{
    public class AutomateT : MonoBehaviour
    {
        [SerializeField] CardManager cardManager;
        [SerializeField] UIManager uiManager;
        
        private enum TestState
        {
            PlayCard,
            ESC,
            RandomOption,
            Reset
        }
        private TestState currentState = TestState.PlayCard;

        private GameManager gameManager;
        private bool isRunningTest = false;
        private float waitTimerForMenu = 0.0f;
        
        private void Start()
        {
            gameManager = GameManager.instance;
            cardManager.trickEnded += ESCFunction;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                isRunningTest = !isRunningTest;
            }

            if (isRunningTest)
            {
                Cursor.lockState = CursorLockMode.Locked;

                switch (currentState)
                {
                    case TestState.PlayCard:
                        waitTimerForMenu = 0.0f;
                        if (cardManager.PlayACard())
                        {
                            currentState = TestState.ESC;
                        }
                        break;
                    
                    case TestState.ESC:
                        break;
                    
                    case TestState.RandomOption:
                        //Wait for animation
                        waitTimerForMenu += Time.deltaTime;
                        if (waitTimerForMenu >= 1.0f)
                        {
                            uiManager.RandomlyClickOnAnOption();
                            currentState = TestState.Reset;
                            waitTimerForMenu = 0.0f;
                        }
                        break;
                    case TestState.Reset:
                        waitTimerForMenu += Time.deltaTime;
                        if (waitTimerForMenu >= 1.0f)
                        {
                            gameManager.TogglePausing();
                            currentState = TestState.PlayCard;
                        }
                        break;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void ESCFunction()
        {
            if (currentState != TestState.ESC) return;
            
            gameManager.TogglePausing();
            currentState = TestState.RandomOption;
        }
    }
}

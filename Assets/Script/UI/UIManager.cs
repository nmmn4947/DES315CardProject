using System;
using UnityEngine;

namespace CardProject
{
    public class UIManager : ActionListManager
    {
        [SerializeField] private MenuButton menuButtonPrefab;
        [SerializeField] private PlayerSetHUD playerHUDTextPrefab;
        [SerializeField] private GameObject menuRoot;
        [SerializeField] private GameObject hudRoot;
        [SerializeField] private GameObject hudPlayer1Object;
        [SerializeField] private GameObject hudPlayer2Object;
        [SerializeField] private GameObject hudPlayer3Object;
        [SerializeField] private GameObject hudPlayer4Object;

        private void Start()
        {
            //Generate Buttons
            
            
            //Generate HUDS
            Instantiate(playerHUDTextPrefab, hudPlayer1Object.transform);
            Instantiate(playerHUDTextPrefab, hudPlayer2Object.transform);
            Instantiate(playerHUDTextPrefab, hudPlayer3Object.transform);
            Instantiate(playerHUDTextPrefab, hudPlayer4Object.transform);
            
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardProject
{
    public class ActionListManager : MonoBehaviour
    {
        public ActionList actionList;
        public float timeMultiplier = 1;
        protected float averageLerpTime = 0.5f;
        
        private void Awake()
        {
            actionList = new ActionList();
        }

        public void LerpTimeMultiplier(float targetTime)
        {
            StartCoroutine(LerpingMultiplier(targetTime));
        }

        IEnumerator LerpingMultiplier(float targetTime)
        {
            float timer = 0;
            float originalTime = timeMultiplier;
            while (averageLerpTime >= timer)
            {
                timer += Time.deltaTime; // will not be effected by other things
                float t = Mathf.Clamp01(timer / averageLerpTime);
                
                timeMultiplier = Mathf.Lerp(originalTime, targetTime, EaseOutCirc(t));
                yield return null;
            }
        }

        private float EaseOutCirc(float t)
        {
            return Mathf.Sqrt(1 - Mathf.Pow(t - 1, 2));
        }
    }
}

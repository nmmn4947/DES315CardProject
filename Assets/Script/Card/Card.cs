using System;
using UnityEngine;

namespace CardProject
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer frontCardRenderer;
        [SerializeField] private SpriteRenderer backCardRenderer;
        private bool isFront;

        private void Update()
        {
            bool isFront = transform.localRotation.eulerAngles.y < 90f
                           || transform.localRotation.eulerAngles.y > 270f;
            frontCardRenderer.sortingOrder = isFront ? 1 : 0;
            backCardRenderer.sortingOrder  = isFront ? 0 : 1;
        }
    }
}

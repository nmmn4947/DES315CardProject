using System;
using UnityEngine;

namespace CardProject
{
    public class MoveToSideOfScreen : MonoBehaviour
    {
        [SerializeField] private bool isLeft;

        private void Awake()
        {
            AUGHH();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            AUGHH();
        }

        private void AUGHH()
        {
            // Get world-space position of left and right screen edges
            Vector3 leftEdgeWorld = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
            Vector3 rightEdgeWorld = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));

            if (isLeft)
            {
                transform.position = new Vector3(leftEdgeWorld.x + 10.0f, transform.position.y, transform.position.z);
            }

            else
            {
                transform.position = new Vector3(rightEdgeWorld.x - 10.0f, transform.position.y, transform.position.z);
            }
        }
    }
}

using UnityEngine;

namespace CardProject
{
    public class ScaleUpSprite : MonoBehaviour
    {
        void Start()
        {
            Camera cam = Camera.main;

            float worldHeight = cam.orthographicSize * 2f;
            float worldWidth = worldHeight * cam.aspect;
            
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            float spriteWidth = sr.bounds.size.x;
            float spriteHeight = sr.bounds.size.y;
            
            Vector3 scale = transform.localScale;

            scale.x = worldWidth / spriteWidth;
            scale.y = worldHeight / spriteHeight;

            transform.localScale = scale;
        }

    }
}

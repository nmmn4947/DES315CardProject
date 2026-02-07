using System;
using UnityEngine;

namespace CardProject
{
    public class MirrorWorldPos : MonoBehaviour
    {
        [SerializeField] private Transform mirrorTransform;
        [SerializeField] private Vector2 mirrorOffset;
        private RectTransform rectTransform;
        private Camera _camera;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            _camera = Camera.main;
        }

        private void Update()
        {
            rectTransform.position = _camera.WorldToScreenPoint(mirrorTransform.position + new Vector3(mirrorOffset.x, mirrorOffset.y, 0));
        }
    }
}

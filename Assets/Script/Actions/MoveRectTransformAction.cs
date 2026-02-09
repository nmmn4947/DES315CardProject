using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace CardProject
{
    public class MoveRectTransformAction : Action
    {
        private RectTransform _rectTransform;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private bool worldPosMove = false;
        
        public MoveRectTransformAction(Vector3 targetPos, GameObject subject, bool blocking, float delay, float duration, Func<float, float> easingFunction) : base(subject, blocking, delay, duration, easingFunction)
        {
            _endPosition = targetPos;
            _rectTransform = subject.GetComponent<RectTransform>();
            if (_rectTransform == null)
            {
                Debug.LogError("RectTransform not found");
            }
            _startPosition = _rectTransform.localPosition;
        }
        
        public MoveRectTransformAction(bool worldPosMovement, Vector3 targetPos, GameObject subject, bool blocking, float delay, float duration, Func<float, float> easingFunction) : base(subject, blocking, delay, duration, easingFunction)
        {
            _endPosition = targetPos;
            _rectTransform = subject.GetComponent<RectTransform>();
            if (_rectTransform == null)
            {
                Debug.LogError("RectTransform not found");
            }
            
            this.worldPosMove = worldPosMovement;
            if (this.worldPosMove)
            {
                _startPosition = _rectTransform.position;
            }
            else
            {
                _startPosition = _rectTransform.localPosition;
            }
        }

        protected override void RunOnceBeforeUpdate()
        {
            if (worldPosMove)
            {
                _startPosition = _rectTransform.position;
            }
            else
            {
                _startPosition = _rectTransform.localPosition;
            }

        }

        protected override bool UpdateLogicUntilDone(float dt)
        {
            
            if (worldPosMove)
            {
                if (duration <= 0.001f)
                {
                    _rectTransform.position = _endPosition;
                    return true;
                }
                _rectTransform.position = Vector3.LerpUnclamped(_startPosition, _endPosition, easingTime);
            }
            else
            {
                if (duration <= 0.001f)
                {
                    _rectTransform.localPosition = _endPosition;
                    return true;
                }
                _rectTransform.localPosition = Vector3.LerpUnclamped(_startPosition, _endPosition, easingTime);
            }
            
            return (timePasses > duration);
        }
    }
}

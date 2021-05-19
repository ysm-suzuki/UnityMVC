using UnityEngine;

namespace UnityMVC
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private float MaxClickPositionDistance = 64;
        [SerializeField]
        private float MaxClickDurationSecond = 0.5f;

        public delegate void EventHandler();
        public event EventHandler OnClicked;
        public event EventHandler OnLongTapped;
        public event EventHandler OnPointerEnter;
        public event EventHandler OnPointerExit;

        public delegate void PositionEventHandler(Vector3 position);
        public event PositionEventHandler OnTouchBegan;
        public event PositionEventHandler OnSecondaryTouchBegan;

        public delegate void DragEventHandler(Vector3 position, float duration);
        public event DragEventHandler OnTouchMoved;
        public event DragEventHandler OnSecondaryTouchMoved;

        public delegate void HoldEventHandler(Vector3 position, float duration, float distance);
        public event HoldEventHandler OnTouchFinished;
        public event HoldEventHandler OnSecondaryTouchFinished;

        public bool relayAllTouchEvents = false;
        public bool relayTouchMoveEvents = false;
        public bool relayClickEvents = false;

        private Vector3 _originalTouchPoint = Vector3.zero;
        private Vector3 _oldTouchPoint = Vector3.zero;
        private float _touchDurationSecond = 0;

        // ========================== called by input porter
        public void BeginTouching(Vector3 position, bool isSecondary = false)
        {
            if (isSecondary)
            {
                if (OnSecondaryTouchBegan != null)
                    OnSecondaryTouchBegan(position);
            }
            else
            {
                if (OnTouchBegan != null)
                    OnTouchBegan(position);
            }

            _originalTouchPoint = position;
            _oldTouchPoint = position;
            _touchDurationSecond = 0;
        }

        public void InTouching(
            Vector3 position,
            float delta,
            bool isSecondary = false,
            bool enableEvent = true)
        {
            if (isSecondary)
            {
                if (OnSecondaryTouchMoved != null
                    && enableEvent)
                    OnSecondaryTouchMoved(position - _oldTouchPoint, delta);
            }
            else
            {
                if (OnTouchMoved != null
                    && enableEvent)
                    OnTouchMoved(position - _oldTouchPoint, delta);
            }

            _oldTouchPoint = position;
            _touchDurationSecond += delta;
        }

        public void FinishTouching(
            Vector3 position,
            float delta,
            bool isSecondary = false,
            bool enableEvent = true)
        {
            _touchDurationSecond += delta;

            if (isSecondary)
            {
                if (OnSecondaryTouchFinished != null
                    && enableEvent)
                    OnSecondaryTouchFinished(position, _touchDurationSecond, (position - _originalTouchPoint).magnitude);
            }
            else
            {
                if (OnTouchFinished != null
                    && enableEvent)
                    OnTouchFinished(position, _touchDurationSecond, (position - _originalTouchPoint).magnitude);
            }

            _oldTouchPoint = position;

            if ((position - _originalTouchPoint).magnitude < MaxClickPositionDistance)
            {
                if (_touchDurationSecond < MaxClickDurationSecond)
                {
                    if (OnClicked != null)
                        OnClicked();
                }
                else
                {
                    if (OnLongTapped != null)
                        OnLongTapped();
                }
            }
        }

        private bool _target = false;
        public bool target
        {
            get
            {
                return _target;
            }
            set
            {
                var enter = !_target && value;
                var exit = _target && !value;
                _target = value;
                if (enter
                    && OnPointerEnter != null) OnPointerEnter();
                if (exit
                    && OnPointerExit != null) OnPointerExit();
            }
        }
    }
}
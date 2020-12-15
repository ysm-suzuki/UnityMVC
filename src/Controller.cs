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

        public delegate void PositionEventHandler(Vector3 position);
        public event PositionEventHandler OnTouchBegan;
        public event PositionEventHandler OnTouchFinished;
        public event PositionEventHandler OnSecondaryTouchBegan;
        public event PositionEventHandler OnSecondaryTouchFinished;

        public delegate void DragEventHandler(Vector3 position, float duration);
        public event DragEventHandler OnTouchMoved;
        public event DragEventHandler OnSecondaryTouchMoved;

        public bool relayTouchEvents = false;

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

        public void InTouching(Vector3 position, float delta, bool isSecondary = false)
        {
            if (isSecondary)
            {
                if (OnSecondaryTouchMoved != null)
                    OnSecondaryTouchMoved(position - _oldTouchPoint, delta);
            }
            else
            {
                if (OnTouchMoved != null)
                    OnTouchMoved(position - _oldTouchPoint, delta);
            }

            _oldTouchPoint = position;
            _touchDurationSecond += delta;
        }

        public void FinishTouching(Vector3 position, float delta, bool isSecondary = false)
        {
            if (isSecondary)
            {
                if (OnSecondaryTouchFinished != null)
                    OnSecondaryTouchFinished(position);
            }
            else
            {
                if (OnTouchFinished != null)
                    OnTouchFinished(position);
            }

            _oldTouchPoint = position;
            _touchDurationSecond += delta;


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
    }
}
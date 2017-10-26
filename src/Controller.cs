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

        public delegate void DragEventHandler(Vector3 position, float duration);
        public event DragEventHandler OnTouchMoved;


        private Vector3 _originalTouchPoint = Vector3.zero;
        private Vector3 _oldTouchPoint = Vector3.zero;
        private float _touchDurationSecond = 0;

        // ========================== called by input porter
        public void BeginTouching(Vector3 position)
        {
            if (OnTouchBegan != null)
                OnTouchBegan(position);

            _originalTouchPoint = position;
            _oldTouchPoint = position;
            _touchDurationSecond = 0;
        }

        public void InTouching(Vector3 position, float delta)
        {
            if (OnTouchMoved != null)
                OnTouchMoved(position - _oldTouchPoint, delta);

            _oldTouchPoint = position;
            _touchDurationSecond += delta;
        }

        public void FinishTouching(Vector3 position, float delta)
        {
            if (OnTouchFinished != null)
                OnTouchFinished(position);

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
using UnityEngine;

namespace UnityMVC
{
    public class TouchInputPorter : InputPorter
    {
        private Controller _currentTarget = null;

        public override void Tick(float delta)
        {
            base.Tick(delta);

            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                var position = touch.position;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        {
                            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(position);
                            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                            if (hit.collider != null)
                            {
                                _currentTarget = hit.collider.gameObject.GetComponent<Controller>();
                                _currentTarget.BeginTouching(position);
                            }
                            break;
                        }
                    case TouchPhase.Moved:
                        {
                            if (_currentTarget != null)
                                _currentTarget.InTouching(position, delta);
                            break;
                        }
                    case TouchPhase.Ended:
                        {
                            if (_currentTarget != null)
                                _currentTarget.FinishTouching(position, delta);

                            _currentTarget = null;
                            break;
                        }
                }
            }
        }
    }
}
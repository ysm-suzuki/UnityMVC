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

                var targets = UpdateTargets(position);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        {
                            if (targets.Count > 0)
                            {
                                _currentTarget = targets[0];
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
            else
            {
                ClearTargets();
            }
        }
    }
}
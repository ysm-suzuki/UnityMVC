using UnityEngine;

namespace UnityMVC
{
    public class MouseInputPorter : InputPorter
    {
        public static int MouseInput1 = 0;
        public static int MouseInput2 = 1;

        private bool _oldClick = false;
        private bool _oldSecondaryClick = false;
        private Controller _currentTarget = null;

        public override void Tick(float delta)
        {
            base.Tick(delta);

            bool currentClick = !_oldSecondaryClick && Input.GetMouseButton(MouseInput1);
            bool secondaryClick = !(currentClick || _oldClick) && Input.GetMouseButton(MouseInput2);
            var position = Input.mousePosition;

            if (currentClick && !_oldClick)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(position);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                if (hit.collider != null)
                {
                    _currentTarget = hit.collider.gameObject.GetComponent<Controller>();
                    _currentTarget.BeginTouching(position);
                }
            }
            else if (currentClick && _oldClick)
            {
                if (_currentTarget != null)
                    _currentTarget.InTouching(position, delta);
            }
            else if (!currentClick && _oldClick)
            {
                if (_currentTarget != null)
                    _currentTarget.FinishTouching(position, delta);

                _currentTarget = null;
            }

            if (secondaryClick && !_oldSecondaryClick)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(position);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                if (hit.collider != null)
                {
                    _currentTarget = hit.collider.gameObject.GetComponent<Controller>();
                    _currentTarget.BeginTouching(position, true);
                }
            }
            else if (secondaryClick && _oldSecondaryClick)
            {
                if (_currentTarget != null)
                    _currentTarget.InTouching(position, delta, true);
            }
            else if (!secondaryClick && _oldSecondaryClick)
            {
                if (_currentTarget != null)
                    _currentTarget.FinishTouching(position, delta, true);

                _currentTarget = null;
            }

            _oldClick = currentClick;
            _oldSecondaryClick = secondaryClick;
        }
    }
}
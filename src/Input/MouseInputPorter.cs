using System.Collections.Generic;
using UnityEngine;

namespace UnityMVC
{
    public class MouseInputPorter : InputPorter
    {
        public static int MouseInput1 = 0;
        public static int MouseInput2 = 1;

        private bool _oldClick = false;
        private bool _oldSecondaryClick = false;
        private List<Controller> _currentTargets = new List<Controller>();

        public override void Tick(float delta)
        {
            base.Tick(delta);

            bool currentClick = !_oldSecondaryClick && Input.GetMouseButton(MouseInput1);
            bool secondaryClick = !(currentClick || _oldClick) && Input.GetMouseButton(MouseInput2);
            var position = Input.mousePosition;

            var targets = UpdateTargets(position);

            if (currentClick && !_oldClick)
            {
                foreach(var target in targets)
                {
                    target.BeginTouching(position);
                    _currentTargets.Add(target);
                }
            }
            else if (currentClick && _oldClick)
            {
                var index = 0;
                foreach (var target in _currentTargets)
                    if(index++ == 0
                        || target.relayTouchMoveEvents)
                        target.InTouching(position, delta);
            }
            else if (!currentClick && _oldClick)
            {
                var index = 0;
                foreach (var target in _currentTargets)
                    if (index++ == 0
                        || target.relayClickEvents)
                        target.FinishTouching(position, delta);
                _currentTargets.Clear();
            }



            if (secondaryClick && !_oldSecondaryClick)
            {
                foreach (var target in targets)
                {
                    target.BeginTouching(position, true);
                    _currentTargets.Add(target);
                }
            }
            else if (secondaryClick && _oldSecondaryClick)
            {
                var index = 0;
                foreach (var target in _currentTargets)
                    if (index++ == 0
                        || target.relayTouchMoveEvents)
                        target.InTouching(position, delta, true);
            }
            else if (!secondaryClick && _oldSecondaryClick)
            {
                var index = 0;
                foreach (var target in _currentTargets)
                    if (index++ == 0
                        || target.relayClickEvents)
                        target.FinishTouching(position, delta, true);
                _currentTargets.Clear();
            }

            _oldClick = currentClick;
            _oldSecondaryClick = secondaryClick;
        }
    }
}
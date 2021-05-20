using System.Collections.Generic;

using UnityEngine;

namespace UnityMVC
{
    public class InputPorter
    {
        public InputPorter() {
            _mainCamera = Camera.main;
        }

        protected Camera _mainCamera;

        virtual public void Tick(float delta)
        {

        }

        private List<Controller> _lastTargets = new List<Controller>();
        protected List<Controller> UpdateTargets(Vector3 position)
        {
            var result = new List<Controller>();

            Vector2 worldPoint = _mainCamera.ScreenToWorldPoint(position);
            RaycastHit2D[] hits = new RaycastHit2D[32];
            Physics2D.RaycastNonAlloc(worldPoint, Vector2.zero, hits);

            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    var target = hit.collider.gameObject.GetComponent<Controller>();
                    target.target = true;
                    result.Add(target);                }
                else
                {
                    break;
                }
            }

            foreach(var lastTarget in _lastTargets)
            {
                if (!result.Contains(lastTarget))
                {
                    lastTarget.target = false;
                }
            }
            _lastTargets.Clear();
            _lastTargets.AddRange(result);

            return result;
        }

        protected void ClearTargets()
        {
            foreach (var lastTarget in _lastTargets)
            {
                lastTarget.target = false;
            }
            _lastTargets.Clear();
        }
    }
}
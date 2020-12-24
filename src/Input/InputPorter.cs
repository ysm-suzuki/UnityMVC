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
    }
}
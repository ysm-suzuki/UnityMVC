using System.Collections.Generic;
using UnityEngine;

namespace UnityMVC
{
    public class View : MonoBehaviour
    {
        [SerializeField]
        private GameObject _root = null;
        [SerializeField]
        protected Controller _controller = null;
        [SerializeField]
        protected Animation _animation = null;

        virtual public void Initialize()
        {
            isDetached = false;
            model = null;
        }

        public GameObject GetRoot()
        {
            return _root != null
                ? _root
                : gameObject;
        }

        virtual public void Detach()
        {
            if (isDetached)
                return;

            if (model != null)
                model.OnPositionUpdated -= UpdatePosition;

            foreach (var view in _linkedViews)
            {
                view.Detach();
            }

            GameObject.Destroy(GetRoot());
            isDetached = true;
        }

        public static T Attach<T>(string prefabPath) where T : View
        {
            var prefab = Resources.Load<GameObject>(prefabPath);
            Debug.Assert(prefab != null, "prefab : " + prefabPath + " not found.");
            var gameObject = GameObject.Instantiate(prefab);
            Debug.Assert(gameObject != null, "prefab : " + prefabPath + " failed in Instantiate().");
            var view = gameObject.GetComponent<T>();
            Debug.Assert(view != null, "prefab : " + prefabPath + " has no view conponents.");
            view.Initialize();
            return view;
        }

        public static T Attach<T>(string prefabPath, GameObject parent) where T : View
        {
            var view = Attach<T>(prefabPath);
            view.SetParent(parent);
            return view;
        }

        public void SetParent(GameObject parent)
        {
            RectTransform rectTransform = GetRoot().GetComponent<RectTransform>();

            var position = GetRoot().transform.localPosition;
            var rotation = GetRoot().transform.localRotation;
            var scale = GetRoot().transform.localScale;

            Vector2 anchoredPosition = Vector2.zero;
            Vector2 sizeDelta = Vector2.zero;
            if (rectTransform != null)
            {
                anchoredPosition = rectTransform.anchoredPosition;
                sizeDelta = rectTransform.sizeDelta;
            }

            GetRoot().transform.SetParent(parent.transform);

            GetRoot().transform.localPosition = position;
            GetRoot().transform.localRotation = rotation;
            GetRoot().transform.localScale = scale;

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = sizeDelta;
            }
        }


        public void SetModel<T>(T model) where T : Model
        {
            this.model = model;
            model.OnPositionUpdated += UpdatePosition;
        }


        protected void UpdatePosition()
        {
            GetRoot().transform.localPosition = model.position.ToVector3();
        }



        // ======================================= linker
        protected List<View> _linkedViews = new List<View>();
        public void Link(View targtet)
        {
            _linkedViews.Add(targtet);
        }

        // ======================================== accessors
        public Model model
        {
            get; protected set;
        }
        public bool isDetached
        {
            get; protected set;
        }

        // ======================================== animation
        private AnimationCallCenter _animationCallCenter = new AnimationCallCenter("OnAnimationFinished");

        virtual public void PlayAnimation(string animationName, System.Action finishCallback = null)
        {
            if (_animation == null)
                return;

            _animation.Stop();

            _animation.cullingType = AnimationCullingType.AlwaysAnimate;
            _animation.Play(animationName, PlayMode.StopSameLayer);

            if (finishCallback != null)
                _animationCallCenter.Register(_animation[animationName], finishCallback);
        }

        // called by animation event
        private void OnAnimationFinished(string animationName)
        {
            _animationCallCenter.Call(animationName);
        }
    }
}

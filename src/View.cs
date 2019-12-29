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

            DepthManager.Unregister(GetRoot());

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
            DepthManager.Register(view.GetRoot());
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


        // ======================================= depth
        private static readonly float DepthMargin = 0.15f;
        private static UIDepthManager DepthManager = new UIDepthManager();
        public void MoveToNearest()
        {
            var nearestZ = DepthManager.nearestZ;
            nearestZ -= DepthMargin;
            var currentPosition = GetRoot().gameObject.transform.position;
            GetRoot().gameObject.transform.position = new Vector3(
                currentPosition.x,
                currentPosition.y,
                nearestZ);
        }

        public static void MoveThemToNearest(List<View> views)
        {
            var nearestZ = DepthManager.nearestZ;
            nearestZ -= DepthMargin;
            foreach (var view in views)
            {
                var currentPosition = view.GetRoot().gameObject.transform.position;
                view.GetRoot().gameObject.transform.position = new Vector3(
                    currentPosition.x,
                    currentPosition.y,
                    nearestZ);
            }
        }

        public static void ClearDepthManager(bool logRemainingTargets = false)
        {
            DepthManager.Clear(logRemainingTargets);
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



    public class UIDepthManager
    {
        private List<GameObject> _targets = new List<GameObject>();
        private List<string> _targetNames = new List<string>();


        public void Register(GameObject target)
        {
            if (_targets.Contains(target)) return;
            _targets.Add(target);

            if (!_targetNames.Contains(target.name))
            {
                _targetNames.Add(target.name);
            }
        }

        public void Unregister(GameObject target)
        {
            if (!_targets.Contains(target)) return;
            _targets.Remove(target);
            if (_targetNames.Contains(target.name))
            {
                _targetNames.Remove(target.name);
            }
        }

        public List<string> GetRegisteredTargetNames()
        {
            var result = new List<string>();
            foreach(var name in _targetNames)
            {
                result.Add(name);
            }
            return result;
        }

        public void Clear(bool logRemainingTargets = false)
        {
            if (logRemainingTargets)
            {
                LogRemainingTargets();
            }

            _targets = new List<GameObject>();
        }

        public void LogRemainingTargets()
        {
            if (GetRegisteredTargetNames().Count == 0) return;
            var log = "";
            foreach (var name in GetRegisteredTargetNames())
            {
                log += name + "\n";
            }
            Debug.Log(log);
        }

        public float nearestZ
        {
            get
            {
                float nearest = float.MaxValue;
                foreach (var target in _targets)
                {
                    var z = target.transform.position.z;
                    if (nearest > z) nearest = z;
                }
                return nearest;
            }
        }
    }
}

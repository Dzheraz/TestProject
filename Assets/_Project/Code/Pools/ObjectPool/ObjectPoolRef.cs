using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DCFApixels
{
    [CreateAssetMenu(fileName = nameof(ObjectPoolRef), menuName = "Pools/" + nameof(ObjectPoolRef), order = 1)]
    public class ObjectPoolRef : ScriptableObject, IObjectPool
    {
        #region Fields
        [SerializeField]
        private ObjectPool _prefab;
        #endregion

        #region Properties
        public ObjectPoolUnit Prefab => _prefab.Prefab;

        #endregion

        #region SingletonLike
        private ObjectPool _instance;
        public void SetInstance_Editor(ObjectPool instance)
        {
            _instance = instance;
        }
        public ObjectPool Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_EDITOR
                    _instance = FindObjectsOfType<ObjectPool>(true).SingleOrDefault(o => o.Ref == this);
                    if (_instance != null)
                    {
                        return _instance;
                    }

                    if (Application.isPlaying)
                    {
                        _instance = Instantiate(_prefab);
                    }
                    else
                    {
                        _instance = (PrefabUtility.InstantiatePrefab(_prefab.gameObject) as GameObject).GetComponent<ObjectPool>();
                    }
#else
                    _instance = Instantiate(_prefab);
#endif
                }
                return _instance;
            }
        }
        #endregion

        #region UnityEvents
        private void OnEnable()
        {
            m_OnAnyTaked = new OnAnyTakedEvent(this);
            m_OnAnyReturned = new OnAnyReturnedEvent(this);
        }
        #endregion

        #region IObjectPool
        public bool IsCanTake => Instance.IsCanTake;

        #region Take
        public T TakeAggressive<T>() where T : Component => Instance.TakeAggressive<T>();
        public ObjectPoolUnit TakeAggressive() => Instance.TakeAggressive();
        public bool TryTake<T>(out T instance) where T : Component => Instance.TryTake(out instance);
        public bool TryTake(out ObjectPoolUnit instance) => Instance.TryTake(out instance);
        public T Take<T>() where T : Component => Instance.Take<T>();
        public ObjectPoolUnit Take() => Instance.Take();
        #endregion

        #region Return
        public void Return(ObjectPoolUnit unit) => Instance.Return(unit);
        #endregion

        public void Destroy() => Instance.Destroy();
        #endregion

        #region PoolEvents
        private OnAnyTakedEvent m_OnAnyTaked;
        private OnAnyReturnedEvent m_OnAnyReturned;

        public OnAnyTakedEvent OnAnyTaked => m_OnAnyTaked;
        public OnAnyReturnedEvent OnAnyReturned => m_OnAnyReturned;

        public struct OnAnyTakedEvent
        {
            private ObjectPoolRef m_Parent;
            public OnAnyTakedEvent(ObjectPoolRef parent) => m_Parent = parent;

            public void Add(ObjectPoolEventHandler listener) => m_Parent.Instance.OnAnyTaked += listener;
            public void Remove(ObjectPoolEventHandler listener) => m_Parent.Instance.OnAnyTaked -= listener;
        }

        public struct OnAnyReturnedEvent
        {
            private ObjectPoolRef m_Parent;
            public OnAnyReturnedEvent(ObjectPoolRef parent) => m_Parent = parent;

            public void Add(ObjectPoolEventHandler listener) => m_Parent.Instance.OnAnyReturned += listener;
            public void Remove(ObjectPoolEventHandler listener) => m_Parent.Instance.OnAnyReturned -= listener;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        internal ObjectPool Instance_Editor => _instance;

        internal void SetPrefab_Editor(ObjectPool prefab)
        {
            _prefab = prefab;
        }
#endif
        #endregion
    }
}
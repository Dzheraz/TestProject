using System;
using System.Linq;
using UnityEngine;

namespace DCFApixels
{
    [DisallowMultipleComponent]
    public class ObjectPoolUnit : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region Editor
#if UNITY_EDITOR
        protected void SetRefs()
        {
            _callbackListeners = GetComponentsInChildren<MonoBehaviour>(true).Where(o => o is IPoolUnitCallbacks && o.GetComponentInParent<ObjectPoolUnit>() == this).ToArray();
        }
        protected void OnValidate()
        {
            _callbackListeners = _callbackListeners.Where(o => o != null && o is IPoolUnitCallbacks).ToArray();
        }
#endif
        #endregion

        #region Fields
        [SerializeField]
        private MonoBehaviour[] _callbackListeners = Array.Empty<MonoBehaviour>();
        private Action _onTaked = delegate { };
        private Action _onReturned = delegate { };

        private ObjectPool _sourcePool;
        internal bool _isInPool;
        #endregion

        #region Properties
        public bool IsInPool => _isInPool;
        #endregion

        #region ReturnToPool
        public void ReturnToPool()
        {
            _sourcePool.Return(this);
        }
        #endregion

        #region Add/Remove Listeners
        public void AddListener(IPoolUnitCallbacks callbacks)
        {
            _onTaked += callbacks.OnTaked;
            _onReturned += callbacks.OnReturned;
        }
        public void RemoveListener(IPoolUnitCallbacks callbacks)
        {
            _onTaked -= callbacks.OnTaked;
            _onReturned -= callbacks.OnReturned;
        }
        public void ClearListeners()
        {
            _onTaked = delegate { };
            _onReturned = delegate { };
        }
        #endregion

        #region ISerializationCallbackReceiver
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            for (int i = 0; i < _callbackListeners.Length; i++)
            {
#if UNITY_EDITOR
                IPoolUnitCallbacks poolUnit = _callbackListeners[i] as IPoolUnitCallbacks;
                if (poolUnit == null)
                    continue;
#else
                IPoolUnitCallbacks poolUnit = (IPoolUnitCallbacks)_callbackListeners[i];
#endif
                AddListener(poolUnit);
            }
        }
        #endregion

        #region Internal
        internal ObjectPool SourcePool { get => _sourcePool; set => _sourcePool = value; }
        internal void CallTaked()
        {
            _onTaked.Invoke();
        }
        internal void CallReturned()
        {
            _onReturned.Invoke();
        }
        #endregion

        #region As
        public T As<T>() where T : ObjectPoolUnit
        {
            return this as T;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        internal void SetRefs_Editor()
        {
            SetRefs();
        }
#endif
        #endregion
    }
}

#region Editor
#if UNITY_EDITOR
namespace DCFApixels.Editors
{
    using System.Reflection;
    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ObjectPoolUnit), true)]
    public class ObjectPoolUnitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ObjectPoolUnit target = this.target as ObjectPoolUnit;
            EditorGUILayout.Toggle("Is In Pool", target.IsInPool);
            if (GUILayout.Button("SetRefs"))
            {
                foreach (var item in targets)
                {
                    item.GetType().GetMethod("SetRefs", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, null);
                }
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif
#endregion
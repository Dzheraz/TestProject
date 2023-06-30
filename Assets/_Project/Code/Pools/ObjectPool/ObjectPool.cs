using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DCFApixels
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class ObjectPool : MonoBehaviour, IObjectPool
    {
        #region Editor
#if UNITY_EDITOR
        private void OnTransformChildrenChanged()
        {
            Validate(GetComponentsInChildren<ObjectPoolUnit>(true).Concat(FindObjectsOfType<ObjectPoolUnit>(true)));
        }

        private void OnValidate()
        {
            Validate(_allUnits);
        }

        private void Validate(IEnumerable<ObjectPoolUnit> list)
        {
            if (Application.isPlaying)
                return;

            var validate = list.Where(o =>
                       o != null &&
                       o.transform.parent == transform &&
                       PrefabUtility.GetCorrespondingObjectFromOriginalSource(o) == _prefab &&
                       o != _prefab).Distinct().ToArray();

            _allUnits.Clear();
            _allUnits.AddRange(validate);
            _disabledUnits.Clear();
            _disabledUnits.AddRange(validate);

            for (int i = 0; i < _allUnits.Count; i++)
            {
                _allUnits[i].name = GetName(i);
                _allUnits[i].gameObject.SetActive(false);
            }
        }

        private void ReValidate()
        {
            if (_prefab == null && transform.childCount > 0)
            {
                ObjectPoolUnit[] childunits = GetComponentsInChildren<ObjectPoolUnit>(true);
                ObjectPoolUnit childunit = null;
                foreach (var unit in childunits)
                {
                    if (unit.transform.parent == transform)
                    {
                        childunit = unit;
                        break;
                    }
                }
                if (childunit != null)
                {
                    _prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(childunit);
                }
            }

            if (GetComponentsInChildren<ObjectPoolUnit>(true).Length > 0)
                Validate(GetComponentsInChildren<ObjectPoolUnit>(true).Concat(FindObjectsOfType<ObjectPoolUnit>(true)));
            else
                Validate(_allUnits);
        }

        internal void ReValidate_Editor()
        {
            ReValidate();
        }

        internal void Revert_Editor()
        {
            foreach (var unit in _allUnits)
            {
                bool isActive = unit.gameObject.activeSelf;
                PrefabUtility.RevertPrefabInstance(unit.gameObject, InteractionMode.AutomatedAction);
                unit.gameObject.SetActive(isActive);
            }
        }

        internal void SetPrefab_Editor(ObjectPoolUnit prefab)
        {
            _prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(prefab);
        }
        internal void SetRef_Editor(ObjectPoolRef @ref)
        {
            _ref = @ref;
        }
#endif
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private List<ObjectPoolUnit> _allUnits;
        [SerializeField, HideInInspector]
        private List<ObjectPoolUnit> _disabledUnits;
        [SerializeField]
        private ObjectPoolRef _ref;
        [SerializeField]
        private ObjectPoolUnit _prefab;
        [SerializeField]
        private int _maxInstances = -1;

        [SerializeField]
        private bool _isAutoCleaning;
        [SerializeField]
        private int _minInstances;
        #endregion

        #region Properties
        public ObjectPoolRef Ref => _ref;
        public int Used => _allUnits.Count - _disabledUnits.Count;
        public ObjectPoolUnit Prefab => _prefab;
        public bool IsAutoCleaning => _isAutoCleaning;
        #endregion

        #region UnityEvents
        private void Awake()
        {
            if (_allUnits == null)
            {
                _allUnits = new List<ObjectPoolUnit>();
                _disabledUnits = new List<ObjectPoolUnit>();
                return;
            }

            foreach (var unit in _allUnits)
            {
                unit.SourcePool = this;
                unit._isInPool = false;
            }
            foreach (var unit in _disabledUnits)
            {
                unit._isInPool = true;
            }
        }
        #endregion

        #region IObjectPool
        public bool IsCanTake => _maxInstances < 0 || _allUnits.Count < _maxInstances || _disabledUnits.Count > 0;

        #region Take
        public T TakeAggressive<T>() where T : Component
        {
            return TakeInternal<T>();
        }
        public ObjectPoolUnit TakeAggressive()
        {
            return TakeInternal();
        }

        public bool TryTake<T>(out T instance) where T : Component
        {
            if (IsCanTake)
            {
                instance = TakeInternal<T>();
                return true;
            }
            else
            {
                instance = null;
                return false;
            }
        }
        public bool TryTake(out ObjectPoolUnit instance)
        {
            if (IsCanTake)
            {
                instance = TakeInternal();
                return true;
            }
            else
            {
                instance = null;
                return false;
            }
        }

        public T Take<T>() where T : Component
        {
            if (IsCanTake == false)
                throw new Exception();

            return TakeInternal<T>();
        }
        public ObjectPoolUnit Take()
        {
            if (IsCanTake == false)
                throw new Exception();

            return TakeInternal();
        }
        #endregion

        #region Return
        public void Return(ObjectPoolUnit unit)
        {
#if DEBUG
            if (unit.SourcePool != this)
                throw new Exception("Attempting to return an object not in the source pool");

#endif
            if (unit.IsInPool == false)
            {
                unit._isInPool = true;
                _disabledUnits.Add(unit);
                unit.gameObject.SetActive(false);
                unit.transform.parent = transform;

                unit.CallReturned();
                OnAnyReturned(this, unit);
            }
        }
        #endregion

        public void Destroy()
        {
            foreach (var unit in _allUnits)
            {
                Destroy(unit.gameObject);
            }
            Destroy(gameObject);
        }
        #endregion

        #region TakeInternal/New
        private T TakeInternal<T>() where T : Component
        {
            ObjectPoolUnit unit = TakeInternal();
            if (typeof(T).IsSubclassOf(typeof(ObjectPoolUnit)))
                return unit as T;
            else
                return unit.GetComponent<T>();
        }
        private ObjectPoolUnit TakeInternal()
        {
            if (_disabledUnits.Count <= 0)
                AddNewInstance();

            int index = _disabledUnits.Count - 1;
            ObjectPoolUnit unit = _disabledUnits[index];
            _disabledUnits.RemoveAt(index);
            unit._isInPool = false;
            unit.CallTaked();
            OnAnyTaked(this, unit);
            unit._isInPool = false;
            return unit;
        }

        private void AddNewInstance()
        {
            ObjectPoolUnit unit;
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                unit = Instantiate(_prefab);
            }
            else
            {
                unit = (PrefabUtility.InstantiatePrefab(_prefab.gameObject) as GameObject).GetComponent<ObjectPoolUnit>();
            }
#else
            unit = Instantiate(_prefab);
#endif
            unit.gameObject.SetActive(false);
            unit.transform.parent = transform;
            unit.name = GetName(_allUnits.Count);
            _allUnits.Add(unit);
            _disabledUnits.Add(unit);

            unit.SourcePool = this;
            unit._isInPool = true;
        }

        private string GetName(int index) => _prefab.name + "_" + index;
        #endregion

        public event ObjectPoolEventHandler OnAnyTaked = delegate { };
        public event ObjectPoolEventHandler OnAnyReturned = delegate { };
    }

    public delegate void ObjectPoolEventHandler(ObjectPool pool, ObjectPoolUnit unit);
}

#region Editor
#if UNITY_EDITOR
namespace DCFApixels.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ObjectPool))]
    public class ObjectPoolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ObjectPool target = this.target as ObjectPool;
            var allUnitsProp = serializedObject.FindProperty("_allUnits");
            var disabledUnitsProp = serializedObject.FindProperty("_disabledUnits");
            var maxInstancesProp = serializedObject.FindProperty("_maxInstances");

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Instances: " + (allUnitsProp.hasMultipleDifferentValues ? "--" : allUnitsProp.arraySize.ToString()), EditorStyles.miniLabel);
            if (allUnitsProp.hasMultipleDifferentValues || disabledUnitsProp.hasMultipleDifferentValues)
                GUILayout.Label("Used: --");
            else
                GUILayout.Label("Used: " + (allUnitsProp.arraySize - disabledUnitsProp.arraySize), EditorStyles.miniLabel);

            GUILayout.Label("Max: " + (maxInstancesProp.hasMultipleDifferentValues ? "--" : maxInstancesProp.intValue < 0 ? "Infinity" : maxInstancesProp.intValue.ToString()), EditorStyles.miniLabel);

            DrawAutoCleaningInfo();

            GUILayout.EndVertical();

            if (GUILayout.Button("ReValidate"))
            {
                target.ReValidate_Editor();
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("Revert"))
            {
                target.Revert_Editor();
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawAutoCleaningInfo()
        {
            ObjectPool target = this.target as ObjectPool;
            var autoCleaningProp = serializedObject.FindProperty("_isAutoCleaning");
            var minInstancesProp = serializedObject.FindProperty("_minInstances");

            if (!autoCleaningProp.boolValue)
            {
                GUILayout.Label("AutoClean: Off", EditorStyles.miniLabel);
                return;
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("AutoClean: On", EditorStyles.miniLabel);
            GUILayout.Label("MinInstances: " + minInstancesProp.intValue, EditorStyles.miniLabel);

            GUILayout.EndVertical();
        }
    }
}
#endif
#endregion
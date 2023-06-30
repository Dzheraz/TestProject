using DCFApixels.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DCFApixels
{
    public class ClassPool<TUnit> : IClassPool<TUnit> where TUnit : class, IClassPoolUnit<TUnit>
    {
        public const int MIN_CAPACITY = 8;

        protected ClassPool(Func<TUnit> instantiateCallback)
        {
            _instantiateCallback = instantiateCallback;
        }
        private Func<TUnit> _instantiateCallback;

        private List<TUnit> _allUnits;
        private List<TUnit> _disabledUnits;
        private int _maxInstances = -1;

        public int Used => _allUnits.Count - _disabledUnits.Count;

        protected static object _look = new object();

        #region IPool
        public bool IsCanTake => _maxInstances < 0 || _allUnits.Count < _maxInstances || _disabledUnits.Count > 0;

        #region Take
        public TUnit TakeAggressiveSafe()
        {
            lock (_look)
            {
                return Take();
            }
        }
        public TUnit TakeAggressive()
        {
            return TakeInternal();
        }
        public bool TryTakeSafe(out TUnit instance)
        {
            lock (_look)
            {
                return TryTake(out instance);
            }
        }
        public bool TryTake(out TUnit instance)
        {
            if (IsCanTake)
            {
                instance = TakeInternal();
                return true;
            }
            else
            {
                instance = null;
                return true;
            }
        }
        public TUnit TakeSafe()
        {
            lock (_look)
            {
                return Take();
            }
        }
        public TUnit Take()
        {
            if (IsCanTake == false)
                throw new Exception();

            return TakeInternal();
        }
        #endregion

        #region Return
        public void Return(TUnit unit)
        {
#if DEBUG
            if (unit.SourcePool != this)
                throw new Exception("Attempting to return an object not in the source pool");

#endif
            if (!unit.IsInPool)
            {
                unit.IsInPool = true;
                _disabledUnits.Add(unit);
                unit.OnReturned();
                OnAnyReturned(this, unit);
            }
        }
        #endregion
        #endregion

        protected TUnit TakeInternal()
        {
            if (_disabledUnits.Count <= 0)
                AddNewInstance();

            int index = _disabledUnits.Count - 1;
            TUnit unit = _disabledUnits[index];
            unit.SourcePool = this;
            _disabledUnits.RemoveAt(index);
            unit.OnTaked();
            OnAnyTaked(this, unit);
            return unit;
        }

        private void AddNewInstance()
        {
            TUnit result = _instantiateCallback();

            _allUnits.Add(result);
            _disabledUnits.Add(result);
        }

        #region Builder
        public static Builder New(Func<TUnit> instantiateCallback)
        {
            return new Builder(new ClassPool<TUnit>(instantiateCallback));
        }

        public Builder Setup(Func<TUnit> instantiateCallback)
        {
            _instantiateCallback = instantiateCallback;
            return new Builder(this);
        }

        public ref struct Builder
        {
            private ClassPool<TUnit> _pool;

            public Builder(ClassPool<TUnit> pool)
            {
                _pool = pool;
                if (pool._allUnits != null)
                {
                    pool._allUnits.Clear();
                    pool._disabledUnits.Clear();
                }
            }

            public Builder PreInstantiate(int count)
            {
                _pool._allUnits = new List<TUnit>(Mathf.Max(MIN_CAPACITY, count));
                _pool._disabledUnits = new List<TUnit>(Mathf.Max(MIN_CAPACITY, count));
                for (int i = 0; i < count; i++)
                {
                    _pool._allUnits.Add(_pool._instantiateCallback());
                }
                _pool._disabledUnits.AddRange(_pool._allUnits);
                return this;
            }

            public Builder Add(IEnumerable<TUnit> range)
            {
                _pool._allUnits.AddRange(range);
                _pool._disabledUnits.AddRange(range);
                return this;
            }
            public Builder Add(TUnit instance)
            {
                _pool._allUnits.Add(instance);
                _pool._disabledUnits.Add(instance);
                return this;
            }

            public Builder Max(int maxInstances)
            {
                _pool._maxInstances = maxInstances;
                return this;
            }
            public Builder MaxInfinite()
            {
                _pool._maxInstances = -1;
                return this;
            }

            public ClassPool<TUnit> End()
            {
                if (_pool._allUnits == null)
                {
                    _pool._allUnits = new List<TUnit>(MIN_CAPACITY);
                    _pool._disabledUnits = new List<TUnit>(MIN_CAPACITY);
                }
                return _pool;
            }
        }
        #endregion

        public event ClassPoolEventHandler<TUnit> OnAnyTaked = delegate { };
        public event ClassPoolEventHandler<TUnit> OnAnyReturned = delegate { };
    }

    public delegate void ClassPoolEventHandler<TUnit>(ClassPool<TUnit> pool, TUnit unit) where TUnit : class, IClassPoolUnit<TUnit>;
}
using DCFApixels;
using System;

public class ArrayUnit<T> : IClassPoolUnit<ArrayUnit<T>>, IDisposable
{
    private ClassPool<ArrayUnit<T>> _sourcePool;
    private bool _isInPool;

    bool IClassPoolUnit<ArrayUnit<T>>.IsInPool { get => _isInPool; set => _isInPool = value; }
    ClassPool<ArrayUnit<T>> IClassPoolUnit<ArrayUnit<T>>.SourcePool { get => _sourcePool; set => _sourcePool = value; }

    public ArrayUnit(int capacity)
    {
        _array = new T[capacity];
    }

    private T[] _array;
    public T[] Array
    {
        get => _array;
        set => _array = value ?? System.Array.Empty<T>();
    }

    public void ReturnToPool()
    {
        _sourcePool.Return(this);
    }

    void IDisposable.Dispose()
    {
        ReturnToPool();
    }
}

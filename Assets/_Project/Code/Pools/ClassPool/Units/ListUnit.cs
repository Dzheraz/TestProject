using DCFApixels;
using System;
using System.Collections.Generic;

public class ListUnit<T> : IClassPoolUnit<ListUnit<T>>, IDisposable
{
    private ClassPool<ListUnit<T>> _sourcePool;
    private bool _isInPool;

    bool IClassPoolUnit<ListUnit<T>>.IsInPool { get => _isInPool; set => _isInPool = value; }
    ClassPool<ListUnit<T>> IClassPoolUnit<ListUnit<T>>.SourcePool { get => _sourcePool; set => _sourcePool = value; }

    public ListUnit(int capacity)
    {
        _list = new List<T>(capacity);
    }

    private List<T> _list;
    public List<T> List => _list;

    public void ReturnToPool()
    {
        _sourcePool.Return(this);
    }

    void IDisposable.Dispose()
    {
        ReturnToPool();
    }
}

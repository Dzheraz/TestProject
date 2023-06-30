using DCFApixels;
using System;
using System.Collections.Generic;

public class QueueUnit<T> : IClassPoolUnit<QueueUnit<T>>, IDisposable
{
    private ClassPool<QueueUnit<T>> _sourcePool;
    private bool _isInPool;

    bool IClassPoolUnit<QueueUnit<T>>.IsInPool { get => _isInPool; set => _isInPool = value; }
    ClassPool<QueueUnit<T>> IClassPoolUnit<QueueUnit<T>>.SourcePool { get => _sourcePool; set => _sourcePool = value; }

    public QueueUnit(int capacity)
    {
        _list = new Queue<T>(capacity);
    }

    private Queue<T> _list;
    public Queue<T> List => _list;

    public void ReturnToPool()
    {
        _sourcePool.Return(this);
    }

    void IDisposable.Dispose()
    {
        ReturnToPool();
    }
}

namespace DCFApixels
{
    public interface IClassPool<T>
    {
        public bool IsCanTake { get; }

        public T TakeAggressive();
        public bool TryTake(out T instance);
        public T Take();

        public void Return(T unit);
    }
}



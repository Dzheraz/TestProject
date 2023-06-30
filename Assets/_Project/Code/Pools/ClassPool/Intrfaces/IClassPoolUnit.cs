namespace DCFApixels
{
    public interface IClassPoolUnit<TSelf> where TSelf : class, IClassPoolUnit<TSelf>
    {
        public ClassPool<TSelf> SourcePool { get; set; }
        public bool IsInPool { get; set; }
        public void ReturnToPool();
    }

    public interface IClassPoolUnitCallbacks<TSelf> : IClassPoolUnit<TSelf> where TSelf : class, IClassPoolUnit<TSelf>
    {
        public void OnTaked();
        public void OnReturned();
    }
}

namespace DCFApixels.Internal
{
    internal static class IClassPoolUnitExtensions
    {
        public static void OnTaked<TSelf>(this IClassPoolUnit<TSelf> self) where TSelf : class, IClassPoolUnit<TSelf>
        {
            ((IClassPoolUnitCallbacks<TSelf>)self).OnTaked();
        }
        public static void OnReturned<TSelf>(this IClassPoolUnit<TSelf> self) where TSelf : class, IClassPoolUnit<TSelf>
        {
            ((IClassPoolUnitCallbacks<TSelf>)self).OnReturned();
        }
    }
}

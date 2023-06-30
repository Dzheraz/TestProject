namespace DCFApixels
{
    public static class DefaultInstantiator<T> where T : new()
    {
        public static T Do() => new T();
    }
}
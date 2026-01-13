namespace Utils
{
    public abstract class Singleton<T> where T : class
    {
        private static T? _instance = null;
        private static readonly object _lock = new();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= Activator.CreateInstance(typeof(T), true) as T;
                    if (_instance == null)
                    {
                        throw new InvalidOperationException("Could not create instance of type " + typeof(T).FullName);
                    }
                    return _instance;
                }
            }
        }
    }
}

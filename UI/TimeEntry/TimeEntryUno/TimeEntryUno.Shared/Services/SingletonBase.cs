using System;
using System.Reflection;

namespace TimeEntryUno.Shared.Services
{
    public abstract class SingletonBase<T> where T : class
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() =>
            {
                var constructor = typeof(T).GetConstructor(
                    BindingFlags.Instance | 
                    BindingFlags.Public | 
                    BindingFlags.NonPublic, 
                    null, Type.EmptyTypes, null);
                return (T)constructor.Invoke(null);
            });

        public static T Instance => _instance.Value;
    }
}

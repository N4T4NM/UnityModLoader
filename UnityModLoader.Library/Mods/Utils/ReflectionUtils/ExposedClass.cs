using System;
using System.Reflection;

namespace UnityModLoader.Library.Mods.Utils.ReflectionUtils
{
    public class ExposedClass<T>
    {
        public const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        public const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;

        public const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
        public const BindingFlags Public = BindingFlags.Public | BindingFlags.Instance;

        public ExposedClass(T src)
        {
            Source = src;
        }

        public T Source { get; private set; }

        PropertyInfo GetProperty(string name, BindingFlags flags)
        {
            Type type = Source.GetType();
            return type.GetProperty(name, flags);
        }
        FieldInfo GetField(string name, BindingFlags flags)
        {
            Type type = Source.GetType();
            return type.GetField(name, flags);
        }
        MethodInfo GetMethod(string name, BindingFlags flags)
        {
            Type type = Source.GetType();
            return type.GetMethod(name, flags);
        }

        public void PropertySet<TValue>(string name, TValue value, BindingFlags flags)
            => GetProperty(name, flags).SetValue(Source, value, null);
        public TValue PropertyGet<TValue>(string name, BindingFlags flags)
            => (TValue)GetProperty(name, flags).GetValue(Source, null);

        public void FieldSet<TValue>(string name, TValue value, BindingFlags flags)
            => GetField(name, flags).SetValue(Source, value);
        public TValue FieldGet<TValue>(string name, BindingFlags flags)
            => (TValue)GetField(name, flags).GetValue(Source);

        public TValue InvokeMethod<TValue>(string name, object[] parameters, BindingFlags flags)
            => (TValue)GetMethod(name, flags).Invoke(Source, parameters);
    }
}

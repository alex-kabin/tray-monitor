using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TrayMonitor
{
    public class ObjectBuilder
    {
        public T Create<T>(string typeName, params object[] args) {
            Type t = Type.GetType(typeName);
            if (t == null) {
                throw new ApplicationException($"Type '{typeName}' not found");
            }
            return (T)Activator.CreateInstance(t, args);
        }
    }
}

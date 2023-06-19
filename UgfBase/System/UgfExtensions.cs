#region

using Secyud.Ugf.Modularity;
using System.Reflection;

#endregion

namespace System
{
    public static class UgfExtensions
    {
        public static bool IsUgfModule(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(IUgfModule).GetTypeInfo().IsAssignableFrom(type);
        }

        public static void CheckUgfModuleType(Type moduleType)
        {
            if (!IsUgfModule(moduleType))
                throw new ArgumentException(
                    "Given type is not an UGF module: " + moduleType.AssemblyQualifiedName
                );
        }
    }
}
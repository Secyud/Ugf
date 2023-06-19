using System;
using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.DependencyInjection
{
    internal class DependencyConstructor : IDependencyConstructor
    {
        private const BindingFlags ConstructFlag =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly ConstructorInfo _constructorInfo;
        private readonly ParameterInfo[] _parameters;

        public DependencyConstructor(Type type)
        {
            ConstructorInfo ci = type.GetConstructors(ConstructFlag).FirstOrDefault();
            _constructorInfo = ci ?? throw new UgfException($"Cannot find constructor for type {type}");
            _parameters = _constructorInfo.GetParameters();
        }

        public object Construct(IDependencyProvider provider)
        {
            object[] objects = new object[_parameters.Length];
            for (int i = 0; i < _parameters.Length; i++)
                objects[i] = provider.Get(_parameters[i].ParameterType);
            return _constructorInfo.Invoke(objects);
        }
    }
}
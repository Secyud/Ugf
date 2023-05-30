using System.Reflection;

namespace Secyud.Ugf.DependencyInjection
{
    internal class ConstructorDescriptor
    {
        private readonly ConstructorInfo _constructorInfo;
        private readonly ParameterInfo[] _parameters;
        public ConstructorDescriptor(ConstructorInfo constructorInfo)
        {
            _constructorInfo = constructorInfo;
            _parameters = _constructorInfo
                .GetParameters();
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
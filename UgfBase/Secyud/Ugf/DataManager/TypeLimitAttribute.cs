using System;

namespace Secyud.Ugf.DataManager
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeLimitAttribute : Attribute
    {
        public TypeLimitAttribute(Type limitType)
        {
            LimitType = limitType;
        }

        public Type LimitType { get; }
    }
}
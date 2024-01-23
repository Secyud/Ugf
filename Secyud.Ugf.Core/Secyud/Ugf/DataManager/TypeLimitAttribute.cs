using System;

namespace Secyud.Ugf.DataManager
{
    /// <summary>
    /// Limit guid. In data manager, only sub type
    /// of limit type can be set as value.  
    /// </summary>
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
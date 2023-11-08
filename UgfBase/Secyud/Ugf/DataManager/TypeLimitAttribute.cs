using System;

namespace Secyud.Ugf.DataManager
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeLimitAttribute:Attribute
    {
        public TypeLimitAttribute(Type limitType, bool limitChild = false)
        {
            LimitType = limitType;
            LimitChild = limitChild;
        }

        public Type LimitType { get;}
        
        public bool LimitChild { get; }
    }
}
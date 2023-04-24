using System;

namespace Secyud.Ugf.Archiving
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TypeIdAttribute:Attribute
    {
        public readonly Guid Id;
        public TypeIdAttribute(string guid)
        {
            Id = Guid.Parse(guid) ;
        }
    }
}
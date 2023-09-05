using System;

namespace Secyud.Ugf.TypeHandle
{
    public readonly struct TypeStruct : IComparable<TypeStruct>, IComparable
    {
        public readonly Type Type;

        public TypeStruct(
            Type type)
        {
            Type = type;
        }

        public TypeStruct(object o)
        {
            Type = o.GetType();
        }

        public override int GetHashCode() =>
            Type.GetHashCode();

        public int CompareTo(object obj) => GetHashCode() - obj.GetHashCode();

        public int CompareTo(TypeStruct other)
        {
            return GetHashCode() - other.GetHashCode();
        }
    }

    public static class DescriptorExtension
    {
        public static TypeStruct Describe(this Type type)
        {
            return new TypeStruct(type);
        }
    }
}
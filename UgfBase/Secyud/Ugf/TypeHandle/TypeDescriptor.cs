using System;

namespace Secyud.Ugf.TypeHandle
{
    public readonly struct TypeDescriptor : IComparable<TypeDescriptor>, IComparable
    {
        public readonly Type Type;

        public TypeDescriptor(
            Type type)
        {
            Type = type;
        }

        public override int GetHashCode() =>
            Type.GetHashCode();

        public int CompareTo(object obj) => GetHashCode() - obj.GetHashCode();

        public int CompareTo(TypeDescriptor other)
        {
            return GetHashCode() - other.GetHashCode();
        }
    }

    public static class DescriptorExtension
    {
        public static TypeDescriptor Describe(this Type type)
        {
            return new TypeDescriptor(type);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Secyud.Ugf.Resource
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SAttribute : Attribute
    {
        private static readonly Dictionary<Type, PropertyType> Map = new()
        {
            [typeof(bool)] = PropertyType.Bool,
            [typeof(byte)] = PropertyType.UInt8,
            [typeof(ushort)] = PropertyType.UInt16,
            [typeof(uint)] = PropertyType.UInt32,
            [typeof(ulong)] = PropertyType.UInt64,
            [typeof(sbyte)] = PropertyType.Int8,
            [typeof(short)] = PropertyType.Int16,
            [typeof(int)] = PropertyType.Int32,
            [typeof(long)] = PropertyType.Int64,
            [typeof(float)] = PropertyType.Single,
            [typeof(double)] = PropertyType.Double,
            [typeof(decimal)] = PropertyType.Decimal,
            [typeof(string)] = PropertyType.String,
            [typeof(Guid)] = PropertyType.Guid,
        };

        public short ID { get; }
        public bool AlwaysSet { get; }
        public PropertyInfo Info { get; private set; }
        public PropertyType Type { get; private set; }

        public SAttribute(short id, bool alwaysSet = false)
        {
            ID = id;
            AlwaysSet = alwaysSet;
        }

        public void SetPropertyType(PropertyInfo info)
        {
            if (Info is not null) return;
            Info = info;
            Map.TryGetValue(info.PropertyType, out PropertyType type);
            Type = type;
        }
    }
}
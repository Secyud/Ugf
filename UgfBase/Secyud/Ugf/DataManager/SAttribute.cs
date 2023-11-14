using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.DataManager
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SAttribute : Attribute
    {
        public static readonly Dictionary<Type, FieldType> Map = new()
        {
            [typeof(bool)] = FieldType.Bool,
            [typeof(byte)] = FieldType.UInt8,
            [typeof(ushort)] = FieldType.UInt16,
            [typeof(uint)] = FieldType.UInt32,
            [typeof(ulong)] = FieldType.UInt64,
            [typeof(sbyte)] = FieldType.Int8,
            [typeof(short)] = FieldType.Int16,
            [typeof(int)] = FieldType.Int32,
            [typeof(long)] = FieldType.Int64,
            [typeof(float)] = FieldType.Single,
            [typeof(double)] = FieldType.Double,
            [typeof(decimal)] = FieldType.Decimal,
            [typeof(string)] = FieldType.String,
            [typeof(Guid)] = FieldType.Guid,
        };

        public SAttribute(short id = 16384)
        {
            Id = id;
        }

        public FieldInfo Info { get; private set; }
        public Type Belong { get; private set; }
        public FieldType Type { get; private set; }
        public FieldType ElementType { get; private set; }
        public short Id { get; }


        public void SetPropertyType(FieldInfo info, Type belong)
        {
            if (Info is not null) return;
            Map.TryGetValue(info.FieldType, out FieldType type);
            Info = info;
            Type = type;
            Belong = belong;
            if (typeof(IList).IsAssignableFrom(info.FieldType))
            {
                Type elementType = info.FieldType.IsArray
                    ? info.FieldType.GetElementType()
                    : info.FieldType.GetGenericArguments().FirstOrDefault();

                if (elementType is not null)
                {
                    Map.TryGetValue(elementType, out FieldType elementFieldType);
                    ElementType = elementFieldType;
                }
            }
            else
            {
                ElementType = FieldType.InValid;
            }
        }

        public bool ReadOnly => Info.IsInitOnly;

        public object GetValue(object obj)
        {
            return Info.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            Info.SetValue(obj, value);
        }

        private object GetDefault()
        {
            return Info.FieldType.IsValueType ? Activator.CreateInstance(Info.FieldType) : null;
        }
    }
}
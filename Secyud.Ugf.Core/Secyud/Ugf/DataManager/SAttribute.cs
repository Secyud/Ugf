using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.DataManager
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SAttribute : Attribute
    {
        private static readonly DescriptionAttribute DefaultDescription = new(null);

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

        public SAttribute(short priority = 16384,
            SShowType showType = SShowType.Normal)
        {
            Priority = priority;
            ShowType = showType;
        }

        public short Priority { get; }
        public string Name => Info.Name;
        public FieldType Type { get; private set; }
        public SShowType ShowType { get; private set; }
        public FieldInfo Info { get; private set; }
        public Type ElementType { get; private set; }

        private DescriptionAttribute _descriptionAttribute;

        public void SetPropertyType(FieldInfo info)
        {
            if (Info is not null) return;
            Info = info;
            if (typeof(IList).IsAssignableFrom(info.FieldType))
            {
                ElementType = info.FieldType.IsArray
                    ? info.FieldType.GetElementType()
                    : info.FieldType.GetGenericArguments().FirstOrDefault();

                FieldType elementFieldType = GetFieldType(ElementType);
                if (elementFieldType > 0)
                {
                    Type = elementFieldType | FieldType.List;
                }
            }
            else
            {
                ElementType = info.FieldType;
                Type = GetFieldType(info.FieldType);
            }
        }


        public string GetDescription()
        {
            if (_descriptionAttribute is null)
            {
                if (Info is not null)
                {
                    _descriptionAttribute = Info
                        .GetCustomAttribute<DescriptionAttribute>() ?? DefaultDescription;
                }
            }

            return _descriptionAttribute?.Description;
        }

        private static FieldType GetFieldType(Type type)
        {
            FieldType fieldType;

            if (type is null)
            {
                fieldType = FieldType.InValid;
            }
            else if (!Map.TryGetValue(type, out fieldType))
            {
                fieldType = FieldType.Object;
            }

            return fieldType;
        }

        public bool ReadOnly => Info.IsInitOnly;

        public object GetValue(object obj)
        {
            return obj is null ? null : Info.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            Info.SetValue(obj, value);
        }
    }
}
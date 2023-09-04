﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Secyud.Ugf.Archiving;

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

        public DataLevel Level { get;  }
        public EditStyle Style { get; }
        public FieldInfo Info { get; private set; }
        public FieldType Type { get; private set; }
        public Type Belong { get; private set; }

        public SAttribute(DataLevel level = DataLevel.Fst,EditStyle style = EditStyle.Default)
        {
            Level = level;
            Style = style;
        }

        public void SetPropertyType(FieldInfo info,Type belong)
        {
            if (Info is not null) return;
            Map.TryGetValue(info.FieldType, out FieldType type);
            Info = info;
            Type = type;
            Belong = belong;
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
            return Info.FieldType.IsValueType ? 
                Activator.CreateInstance(Info.FieldType) : null;
        }
    }
}
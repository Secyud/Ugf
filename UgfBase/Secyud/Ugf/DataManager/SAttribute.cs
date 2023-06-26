using System;
using System.Collections.Generic;
using System.Reflection;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SAttribute : Attribute
    {
        private static readonly Dictionary<Type, FieldType> Map = new()
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

        public short ID { get; set; }
        public DataType DataType { get; set; }
        public EditStyle Style { get; set; }
        public FieldInfo Info { get; private set; }
        public FieldType Type { get; private set; }

        public void SetPropertyType(FieldInfo info)
        {
            if (Info is not null) return;
            Info = info;
            Map.TryGetValue(info.FieldType, out FieldType type);
            Type = type;
        }

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

        public void Write(object value, IArchiveWriter writer)
        {
            value ??= GetDefault();
            writer.Write(value,Type);
        }

        public object Read(IArchiveReader reader)
        {
            object value = reader.Read(Type);
            value ??= GetDefault();
            return value;
        }
    }
}
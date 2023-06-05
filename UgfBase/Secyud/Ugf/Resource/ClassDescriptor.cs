#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion

namespace Secyud.Ugf.Resource
{
    public class ClassDescriptor
    {
        public readonly ConstructorInfo Constructor;
        public readonly Type Type;
        public readonly SortedDictionary<int, ClassProperty> Properties = new();

        public struct ClassProperty
        {
            public PropertyInfo Info { get; }
            public PropertyType Type { get; }

            public bool NoArchive { get; }

            public ClassProperty(PropertyInfo info, PropertyType type, bool noArchive)
            {
                Info = info;
                Type = type;
                NoArchive = noArchive;
            }
        }

        public object Construct()
        {
            return Constructor?.Invoke(Array.Empty<object>());
        }

        public ClassDescriptor(Type type)
        {
            Type = type;
            Constructor = type.GetConstructor(Og.ConstructFlag, null, Type.EmptyTypes, null);

            if (typeof(ResourcedBase).IsAssignableFrom(type))
            {
                PropertyInfo[] infos = type.GetProperties();
                foreach (PropertyInfo info in infos)
                {
                    RAttribute attribute = info.GetCustomAttribute<RAttribute>();
                    if (attribute is null) continue;
                    Type pType = info.PropertyType;
                    PropertyType t = PropertyType.InValid;
                    if (pType == typeof(bool)) t = PropertyType.Bool;
                    else if (pType == typeof(byte)) t = PropertyType.UInt8;
                    else if (pType == typeof(ushort)) t = PropertyType.UInt16;
                    else if (pType == typeof(uint)) t = PropertyType.UInt32;
                    else if (pType == typeof(ulong)) t = PropertyType.UInt64;
                    else if (pType == typeof(sbyte)) t = PropertyType.Int8;
                    else if (pType == typeof(short)) t = PropertyType.Int16;
                    else if (pType == typeof(int)) t = PropertyType.Int32;
                    else if (pType == typeof(long)) t = PropertyType.Int64;
                    else if (pType == typeof(float)) t = PropertyType.Single;
                    else if (pType == typeof(double)) t = PropertyType.Double;
                    else if (pType == typeof(string)) t = PropertyType.String;
                    else if (pType == typeof(Guid)) t = PropertyType.Guid;

                    if (t == PropertyType.InValid) continue;
                    Properties[attribute.ID] = new ClassProperty(info, t, attribute.NoArchive);
                }
            }
        }

        private static void Write(object o, ClassProperty property, BinaryWriter writer)
        {
            object v = property.Info.GetValue(o);
            switch (property.Type)
            {
                case PropertyType.Bool:
                    writer.Write((bool)v);
                    break;
                case PropertyType.UInt8:
                    writer.Write((byte)v);
                    break;
                case PropertyType.UInt16:
                    writer.Write((ushort)v);
                    break;
                case PropertyType.UInt32:
                    writer.Write((uint)v);
                    break;
                case PropertyType.UInt64:
                    writer.Write((ulong)v);
                    break;
                case PropertyType.Int8:
                    writer.Write((sbyte)v);
                    break;
                case PropertyType.Int16:
                    writer.Write((short)v);
                    break;
                case PropertyType.Int32:
                    writer.Write((int)v);
                    break;
                case PropertyType.Int64:
                    writer.Write((long)v);
                    break;
                case PropertyType.Single:
                    writer.Write((float)v);
                    break;
                case PropertyType.Double:
                    writer.Write((double)v);
                    break;
                case PropertyType.String:
                    writer.Write((string)v);
                    break;
                case PropertyType.Guid:
                    writer.Write((Guid)v);
                    break;
                case PropertyType.InValid:
                    throw new InvalidDataException("Property type is not valid");
              
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Read(object obj, ClassProperty property, BinaryReader reader)
        {
            property.Info.SetValue(obj, property.Type switch
            {
                PropertyType.Bool => reader.ReadBoolean(),
                PropertyType.UInt8 => reader.ReadByte(),
                PropertyType.UInt16 => reader.ReadUInt16(),
                PropertyType.UInt32 => reader.ReadUInt32(),
                PropertyType.UInt64 => reader.ReadUInt64(),
                PropertyType.Int8 => reader.ReadSByte(),
                PropertyType.Int16 => reader.ReadInt16(),
                PropertyType.Int32 => reader.ReadInt32(),
                PropertyType.Int64 => reader.ReadInt64(),
                PropertyType.Single => reader.ReadSingle(),
                PropertyType.Double => reader.ReadDouble(),
                PropertyType.String => reader.ReadString(),
                PropertyType.Guid => reader.ReadGuid(),
                _ => default
            });
        }

        public void Init(object obj, ResourceDescriptor descriptor, bool archiving)
        {
            foreach ((int id, ClassProperty property) in Properties)
            {
                if (archiving && !property.NoArchive)
                    continue;
                property.Info.SetValue(obj, descriptor[id]);
            }
        }

        public void Write(object o, BinaryWriter writer)
        {
            foreach (ClassProperty property in
                     Properties.Values.Where(property => !property.NoArchive))
                Write(o, property, writer);
        }

        public void Read(object obj, BinaryReader reader)
        {
            foreach (ClassProperty property in
                     Properties.Values.Where(property => !property.NoArchive))
                Read(obj, property, reader);
        }

        public void CopyTo(object from, object to)
        {
            foreach (ClassProperty property in Properties.Values)
            {
                object obj = property.Info.GetValue(from);
                property.Info.SetValue(to, obj);
            }
        }
    }
}
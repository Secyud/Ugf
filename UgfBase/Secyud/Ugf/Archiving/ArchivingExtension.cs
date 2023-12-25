using System;
using System.Collections;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.Modularity;
using UnityEngine;

namespace Secyud.Ugf.Archiving
{
    public static class ArchivingExtension
    {
        public static object ReadField(this IArchiveReader reader, FieldType type)
        {
            return type switch
            {
                FieldType.Bool    => reader.ReadBoolean(),
                FieldType.UInt8   => reader.ReadByte(),
                FieldType.UInt16  => reader.ReadUInt16(),
                FieldType.UInt32  => reader.ReadUInt32(),
                FieldType.UInt64  => reader.ReadUInt64(),
                FieldType.Int8    => reader.ReadSByte(),
                FieldType.Int16   => reader.ReadInt16(),
                FieldType.Int32   => reader.ReadInt32(),
                FieldType.Int64   => reader.ReadInt64(),
                FieldType.Single  => reader.ReadSingle(),
                FieldType.Double  => reader.ReadDouble(),
                FieldType.Decimal => reader.ReadDecimal(),
                FieldType.String  => reader.ReadString(),
                FieldType.Guid    => reader.ReadGuid(),
                FieldType.Object  => reader.ReadNullable<object>(),
                _                 => new NotSupportedException("Type not support!")
            };
        }

        public static void WriteField(this IArchiveWriter writer, object value, FieldType type)
        {
            switch (type)
            {
                case FieldType.Bool:
                    writer.Write((bool)value);
                    break;
                case FieldType.UInt8:
                    writer.Write((byte)value);
                    break;
                case FieldType.UInt16:
                    writer.Write((ushort)value);
                    break;
                case FieldType.UInt32:
                    writer.Write((uint)value);
                    break;
                case FieldType.UInt64:
                    writer.Write((ulong)value);
                    break;
                case FieldType.Int8:
                    writer.Write((sbyte)value);
                    break;
                case FieldType.Int16:
                    writer.Write((short)value);
                    break;
                case FieldType.Int32:
                    writer.Write((int)value);
                    break;
                case FieldType.Int64:
                    writer.Write((long)value);
                    break;
                case FieldType.Single:
                    writer.Write((float)value);
                    break;
                case FieldType.Double:
                    writer.Write((double)value);
                    break;
                case FieldType.Decimal:
                    writer.Write((decimal)value);
                    break;
                case FieldType.String:
                    writer.Write((string)value ?? string.Empty);
                    break;
                case FieldType.Guid:
                    writer.Write((Guid)value);
                    break;
                case FieldType.Object:
                    writer.WriteNullable(value);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }


        public static void SaveResource(this IDataResource resource, IArchiveWriter writer)
        {
            writer.Write(resource.ResourceId);
        }

        public static void LoadResource(this IDataResource shown, IArchiveReader reader)
        {
            string resourceId = reader.ReadString();
            U.Tm.LoadObjectFromResource(shown, resourceId);
        }


        public static IEnumerator GameInitialization(this IUgfApplication application)
        {
            using GameInitializeContext context = new(application.DependencyManager.CreateScopeProvider());

            foreach (IUgfModuleDescriptor module in application.Modules)
            {
                if (module.Instance is IOnPreInitialization preInitialization)
                {
                    yield return preInitialization.OnGamePreInitialization(context);
                }
            }

            foreach (var module in application.Modules)
            {
                if (module.Instance is IOnInitialization initialization)
                {
                    yield return initialization.OnGameInitializing(context);
                }
            }

            foreach (IUgfModuleDescriptor module in application.Modules)
            {
                if (module.Instance is IOnPostInitialization postInitialization)
                {
                    yield return postInitialization.OnGamePostInitialization(context);
                }
            }
        }

        public static IEnumerator GameLoading(this IUgfApplication application)
        {
            using GameInitializeContext context = new(application.DependencyManager.CreateScopeProvider());

            foreach (IUgfModuleDescriptor module in application.Modules)
            {
                if (module.Instance is IOnPreInitialization preInitialization)
                {
                    yield return preInitialization.OnGamePreInitialization(context);
                }
            }

            foreach (IUgfModuleDescriptor module in application.Modules)
            {
                if (module.Instance is IOnArchiving initialization)
                {
                    yield return initialization.LoadGame();
                }
            }

            foreach (IUgfModuleDescriptor module in application.Modules)
            {
                if (module.Instance is IOnPostInitialization postInitialization)
                {
                    yield return postInitialization.OnGamePostInitialization(context);
                }
            }
        }

        public static IEnumerator GameSaving(this IUgfApplication applications)
        {
            foreach (IUgfModuleDescriptor descriptor in applications.Modules)
            {
                if (descriptor.Instance is IOnArchiving archiving)
                {
                    yield return archiving.SaveGame();
                }
            }
        }

        public static void InitializeGame(this UgfApplicationFactory factory)
        {
            if (factory.Manager is MonoBehaviour monoBehaviour)
            {
                monoBehaviour.StartCoroutine(factory.Application.GameInitialization());
            }
        }

        public static void SaveGame(this UgfApplicationFactory factory)
        {
            if (factory.Manager is MonoBehaviour monoBehaviour)
            {
                monoBehaviour.StartCoroutine(factory.Application.GameSaving());
            }
        }

        public static void LoadGame(this UgfApplicationFactory factory)
        {
            if (factory.Manager is MonoBehaviour monoBehaviour)
            {
                monoBehaviour.StartCoroutine(factory.Application.GameLoading());
            }
        }
    }
}
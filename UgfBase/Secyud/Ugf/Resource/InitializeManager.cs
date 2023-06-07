using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Secyud.Ugf.Resource
{
    public sealed class InitializeManager : ISingleton
    {
        private SortedDictionary<string, PathNode> _rootNodes = new();
        private readonly Dictionary<Type, Dictionary<string, ResourceDescriptor>> _resource = new();

        public ResourceDescriptor GetOrAddResource(Guid typeId, string name, Type templateType)
        {
            if (!_resource.TryGetValue(templateType, out Dictionary<string, ResourceDescriptor> dict))
            {
                dict = new Dictionary<string, ResourceDescriptor>();
                _resource[templateType] = dict;
            }

            if (!dict.TryGetValue(name, out ResourceDescriptor descriptor))
            {
                descriptor = new ResourceDescriptor(name, typeId,templateType);
                dict[name] = descriptor;
            }

            return descriptor;
        }

        public ResourceDescriptor GetResource(Type templateType, string name)
        {
            if (_resource.TryGetValue(templateType, out Dictionary<string, ResourceDescriptor> dict) &&
                dict.TryGetValue(name, out ResourceDescriptor descriptor))
                return descriptor;
            throw new UgfException($"Cannot get resource for template id {templateType} named {name}");
        }
        public ResourceDescriptor GetResource<TTemplate>(string name)
        {
            return GetResource(typeof(TTemplate), name);
        } 
        
        public List<string> GetResourceList(Type templateType)
        { 
            if (_resource.TryGetValue(templateType, out Dictionary<string, ResourceDescriptor> dict))
                return dict.Keys.ToList();
            throw new UgfException($"Cannot get resource dict for type {templateType}");
        }

        public PathNode GetOrAddPathNode(string path)
        {
            string[] arr = path.Split('/');
            PathNode parentNode = null;
            SortedDictionary<string, PathNode> nodes = _rootNodes;
            foreach (string nodeStr in arr)
            {
                if (!nodes.TryGetValue(nodeStr, out PathNode node))
                {
                    node = new PathNode(nodeStr, parentNode);
                    _rootNodes[nodeStr] = node;
                }

                _rootNodes = node.Child;
                parentNode = node;
            }

            return parentNode;
        }
        //
        // public void RegisterFromCsv(string path)
        // {
        //     if (!File.Exists(path))
        //     {
        //         Debug.LogWarning(
        //             $"{nameof(InitializeManager)}_{nameof(RegisterFromCsv)}: file doesn't exist: {path}!"
        //         );
        //         return;
        //     }
        //
        //     using FileStream file = File.OpenRead(path);
        //     using CsvReader reader = new(file);
        //
        //     CsvRow row = new();
        //     if (!reader.ReadRow(row))
        //         return;
        //
        //     List<CsvResourceLineLabel> labels = new();
        //
        //     for (int i = 0; i < row.Count; i++)
        //     {
        //         string title = row[i];
        //         if (title.IsNullOrWhiteSpace())
        //             continue;
        //         title = title.Trim();
        //         int index = title.IndexOf('_');
        //         if (index == -1 ||
        //             !int.TryParse(title[..index], out int type) ||
        //             !int.TryParse(title[(index + 1)..], out int id))
        //             continue;
        //
        //         labels.AddLast(new CsvResourceLineLabel
        //             { Col = i, Id = id, Type = (PropertyType)type });
        //     }
        //
        //     while (reader.ReadRow(row))
        //     {
        //         string name = row[0].Trim();
        //         if (name.IsNullOrWhiteSpace())
        //             continue;
        //
        //         ResourceDescriptor descriptor = GetOrAddResource(name);
        //
        //         foreach (CsvResourceLineLabel label in labels)
        //         {
        //             string str = row[label.Col].Trim();
        //             if (str.IsNullOrEmpty()) continue;
        //
        //             switch (label.Id)
        //             {
        //                 case -1:
        //                     descriptor[label.Id] = str;
        //                     break;
        //                 case -2:
        //                 {
        //                     if (Guid.TryParse(str, out Guid id))
        //                         descriptor[label.Id] = id;
        //                     break;
        //                 }
        //                 default:
        //                     try
        //                     {
        //                         object value = label.Type switch
        //                         {
        //                             PropertyType.Bool => str == "1",
        //                             PropertyType.UInt8 => byte.Parse(str),
        //                             PropertyType.UInt16 => ushort.Parse(str),
        //                             PropertyType.UInt32 => uint.Parse(str),
        //                             PropertyType.UInt64 => ulong.Parse(str),
        //                             PropertyType.Int8 => sbyte.Parse(str),
        //                             PropertyType.Int16 => short.Parse(str),
        //                             PropertyType.Int32 => int.Parse(str),
        //                             PropertyType.Int64 => long.Parse(str),
        //                             PropertyType.Single => float.Parse(str),
        //                             PropertyType.Double => double.Parse(str),
        //                             PropertyType.String => GetOrAddPathNode(str).Path,
        //                             PropertyType.Guid => Guid.Parse(str),
        //                             PropertyType.InValid => throw new InvalidDataException(),
        //                             _ => throw new ArgumentOutOfRangeException()
        //                         };
        //
        //                         descriptor[label.Id] = value;
        //                     }
        //                     catch (Exception)
        //                     {
        //                         // ignored
        //                     }
        //
        //                     break;
        //             }
        //         }
        //     }
        // }

        public void RegisterFromBinary(string path, Type templateType)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning(
                    $"{nameof(InitializeManager)}_{nameof(RegisterFromBinary)}: file doesn't exist: {path}!"
                );
                return;
            }
            
            if (!_resource.TryGetValue(templateType, out Dictionary<string, ResourceDescriptor> dict))
            {
                dict = new Dictionary<string, ResourceDescriptor>();
                _resource[templateType] = dict;
            }

            using FileStream file = File.OpenRead(path);
            using BinaryReader reader = new(file);

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                Guid id = reader.ReadGuid();
                string name = reader.ReadString();
                int length = reader.ReadInt32();

                if (!dict.TryGetValue(name, out ResourceDescriptor descriptor))
                {
                    descriptor = new ResourceDescriptor(name, id,templateType);
                    dict[name] = descriptor;
                }

                for (int j = 0; j < length; j++)
                {
                    PropertyType type = (PropertyType)reader.ReadByte();
                    int index = reader.ReadInt32();
                    descriptor[index] = type switch
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
                        PropertyType.String => GetOrAddPathNode(reader.ReadString()).Path,
                        PropertyType.Guid => reader.ReadGuid(),
                        PropertyType.InValid => throw new InvalidDataException(),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }
        }
    }
}
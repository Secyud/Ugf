using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

namespace Secyud.Ugf.Resource
{
    public sealed class InitializeManager : ISingleton
    {
        private SortedDictionary<string, PathNode> _rootNodes = new();
        private readonly Dictionary<string, ResourceDescriptor> _resource = new();

        public ResourceDescriptor GetOrAddResource(string name)
        {
            if (!_resource.TryGetValue(name, out ResourceDescriptor descriptor))
            {
                descriptor = new ResourceDescriptor();
                _resource[name] = descriptor;
                descriptor[-1] = Encoding.UTF8.GetBytes(name);
            }

            return descriptor;
        }

        public ResourceDescriptor GetResource(string name)
        {
            return !_resource.TryGetValue(name, out ResourceDescriptor descriptor)
                ? null
                : descriptor;
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

        public void ConfigResource(string name, Action<ResourceDescriptor> action)
        {
            ResourceDescriptor descriptor = GetOrAddResource(name);
            action(descriptor);
        }


        public void RegisterResource(string name, string path, short id)
        {
            ResourceDescriptor descriptor = GetOrAddResource(name);
            PathNode node = GetOrAddPathNode(path);
            descriptor[id] = node.Path;
        }

        public List<string> RegisterFromCsv(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning(
                    $"{nameof(InitializeManager)}_{nameof(RegisterFromCsv)}: file doesn't exist: {path}!"
                );
                return null;
            }

            using FileStream file = File.OpenRead(path);
            using CsvReader reader = new(file);

            CsvRow row = new();
            if (!reader.ReadRow(row))
                return null;

            List<CsvResourceLineLabel> labels = new();

            for (int i = 0; i < row.Count; i++)
            {
                string title = row[i];
                if (title.IsNullOrWhiteSpace())
                    continue;
                title = title.Trim();
                int index = title.IndexOf('_');
                if (index == -1 ||
                    !int.TryParse(title[..index], out int type) ||
                    !int.TryParse(title[(index + 1)..], out int id))
                    continue;

                labels.AddLast(new CsvResourceLineLabel
                    { Col = i, Id = id, Type = (PropertyType)type });
            }

            List<string> names = new();

            while (reader.ReadRow(row))
            {
                string name = row[0].Trim();
                if (name.IsNullOrWhiteSpace())
                    continue;

                ResourceDescriptor descriptor = GetOrAddResource(name);

                foreach (CsvResourceLineLabel label in labels)
                {
                    string str = row[label.Col].Trim();
                    if (str.IsNullOrEmpty()) continue;

                    switch (label.Id)
                    {
                        case -1:
                            descriptor[label.Id] = str;
                            break;
                        case -2:
                        {
                            if (Guid.TryParse(str,out Guid id))
                                descriptor[label.Id] = id;
                            break;
                        }
                        default:
                            try
                            {
                                object value = label.Type switch
                                {
                                    PropertyType.Bool => str == "1",
                                    PropertyType.UInt8 => byte.Parse(str),
                                    PropertyType.UInt16 => ushort.Parse(str),
                                    PropertyType.UInt32 => uint.Parse(str),
                                    PropertyType.UInt64 => ulong.Parse(str),
                                    PropertyType.Int8 => sbyte.Parse(str),
                                    PropertyType.Int16 => short.Parse(str),
                                    PropertyType.Int32 => int.Parse(str),
                                    PropertyType.Int64 => long.Parse(str),
                                    PropertyType.Single => float.Parse(str),
                                    PropertyType.Double => double.Parse(str),
                                    PropertyType.String => GetOrAddPathNode(str).Path,
                                    PropertyType.InValid => throw new InvalidDataException(),
                                    _ => throw new ArgumentOutOfRangeException()
                                };
                            
                                descriptor[label.Id] = value;
                            }
                            catch (Exception)
                            {
                                // ignored
                            }

                            break;
                    }
                }
                names.Add(name);
            }
            return names;
        }
    }
}
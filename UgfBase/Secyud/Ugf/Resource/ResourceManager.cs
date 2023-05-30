using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Secyud.Ugf.Resource
{
	public sealed class ResourceManager : ISingleton
	{
		public static ResourceManager Instance => _instance ??= Og.DefaultProvider.Get<ResourceManager>();

		private static ResourceManager _instance;

		private readonly Dictionary<string, ResourceDescriptor> _resource = new();
		private readonly Dictionary<Type, ResourceProperty> _resourceProperty = new();

		public SortedDictionary<string, ResourceNode> RootNodes = new();

		public ResourceDescriptor GetResourceDescriptor(string name)
		{
			if (!_resource.TryGetValue(name, out ResourceDescriptor descriptor))
			{
				descriptor = new ResourceDescriptor();
				_resource[name] = descriptor;
				descriptor.SConfigs[-1] = CreateOrGetPathNode(name);
			}
			return descriptor;
		}

		public ResourceProperty GetResourceProperty(Type type)
		{
			if (!_resourceProperty.TryGetValue(type, out ResourceProperty property))
			{
				property = new ResourceProperty(type);
				_resourceProperty[type] = property;
			}
			return property;
		}

		public ResourceNode GetResource(string name, short id)
		{
			ResourceDescriptor descriptor = GetResourceDescriptor(name);
			if (descriptor is null) return null;

			descriptor.SConfigs.TryGetValue(id, out ResourceNode resource);
			return resource;
		}

		public void ConfigResource(string name, Action<ResourceDescriptor> action)
		{
			ResourceDescriptor descriptor = GetResourceDescriptor(name);
			action(descriptor);
		}

		public ResourceNode CreateOrGetPathNode(string path)
		{
			string[] arr = path.Split('/');
			ResourceNode parentNode = null;
			SortedDictionary<string, ResourceNode> nodes = RootNodes;
			foreach (string nodeStr in arr)
			{
				if (!nodes.TryGetValue(nodeStr, out ResourceNode node))
				{
					node = new ResourceNode(nodeStr, parentNode);
					RootNodes[nodeStr] = node;
				}
				RootNodes = node.Child;
				parentNode = node;
			}
			return parentNode;
		}

		public void RegisterResource(string name, string path, short id)
		{
			ResourceDescriptor descriptor = GetResourceDescriptor(name);
			descriptor.SConfigs[id] = CreateOrGetPathNode(path);
		}

		public List<string> RegisterFromCsv(string path)
		{
			if (!File.Exists(path))
			{
				Debug.LogWarning(
					$"{nameof(ResourceManager)}_{nameof(RegisterFromCsv)}: file doesn't exist: {path}!"
				);
				return null;
			}

			using FileStream file = File.OpenRead(path);
			using CsvReader reader = new(file);

			CsvRow row = new();
			if (!reader.ReadRow(row))
				return null;

			List<CsvResourceLineLabel> labels = new();
			int titleCol = -1;
			for (int i = 0; i < row.Count; i++)
			{
				if (i > 255) break;

				string title = row[i];
				if (title.IsNullOrWhiteSpace())
					continue;

				title = title.Trim();

				if (title == "Title")
				{
					titleCol = i;
					continue;
				}

				if (short.TryParse(title[1..], out short id))
				{
					labels.AddLast(
						new CsvResourceLineLabel
						{
							Col = (byte)i,
							Id = id,
							Label = title[0] switch
							{
								'S' => 0, 's' => 0,
								'D' => 1, 'd' => 1,
								'F' => 2, 'f' => 2,
								_ => 0
							}
						}
					);
				}
			}

			if (titleCol < 0)
			{
				Debug.LogWarning(
					$"{nameof(ResourceManager)}_{nameof(RegisterFromCsv)}: no title found!"
				);
			}

			List<string> names = new();
			while (reader.ReadRow(row))
			{
				string title = row[titleCol].Trim();
				if (title.IsNullOrWhiteSpace())
					continue;

				ResourceDescriptor descriptor = GetResourceDescriptor(title);

				foreach (CsvResourceLineLabel label in labels)
				{
					if (row.Count > label.Col)
					{
						string value = row[label.Col].Trim();
						if (value.IsNullOrEmpty()) continue;

						switch (label.Label)
						{
						case 0:
							descriptor.SConfigs[label.Id] = CreateOrGetPathNode(value);
							break;
						case 1:
							if (int.TryParse(value, out int i))
								descriptor.DConfigs[label.Id] = i;
							break;
						case 2:
							if (float.TryParse(value, out float f))
								descriptor.FConfigs[label.Id] = f;
							break;
						}
					}
				}
				names.Add(title);
			}
			return names;
		}
	}
}
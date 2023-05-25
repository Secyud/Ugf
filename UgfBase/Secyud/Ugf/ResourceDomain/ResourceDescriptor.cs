using System.Collections.Generic;

namespace Secyud.Ugf.ResourceDomain
{
	public class ResourceDescriptor
	{
		public readonly SortedDictionary<short, ResourceNode> SConfigs = new();
		public readonly SortedDictionary<short, float> FConfigs = new();
		public readonly SortedDictionary<short, int> DConfigs = new();

		public string S(short id)
		{
			SConfigs.TryGetValue(id, out ResourceNode node);
			return node?.Path;
		}

		public float F(short id)
		{
			FConfigs.TryGetValue(id, out float node);
			return node;
		}

		public int D(short id)
		{
			DConfigs.TryGetValue(id, out int node);
			return node;
		}
	}
}
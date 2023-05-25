using System.Collections.Generic;

namespace Secyud.Ugf.Resource
{
	public class ResourceNode
	{
		public readonly ResourceNode Parent;

		public readonly string NodeName;

		public readonly SortedDictionary<string, ResourceNode> Child = new();

		private string _path = null;

		public ResourceNode(string nodeName,ResourceNode parent)
		{
			NodeName = nodeName;
			if (parent is not null)
			{
				Parent = parent;
				Parent.Child[nodeName] = this;
			}
		}

		public string Path
		{
			get
			{
				if (_path is null)
				{
					if (Parent is null)
						_path = NodeName;
					else
						_path = Parent.Path + "/" + NodeName;
				}
				return _path;
			}
		}
		
	}
}
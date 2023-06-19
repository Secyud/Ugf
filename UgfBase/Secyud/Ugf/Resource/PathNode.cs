using System.Collections.Generic;

namespace Secyud.Ugf.Resource
{
    public class PathNode
    {
        public readonly PathNode Parent;

        public readonly string NodeName;

        public readonly SortedDictionary<string, PathNode> Child = new();

        private string _path = null;

        public PathNode(string nodeName, PathNode parent)
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
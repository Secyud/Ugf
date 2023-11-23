using System.Collections.Generic;

namespace Secyud.Ugf.TreeViewComponents
{
    public interface ITreeNode
    {
        public bool Collapsed { get; set; }

        public IList<ITreeNode> SubNodes { get; }

        public void SetNode(TreeNodeView node);
    }
}
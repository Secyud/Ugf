using System.Ugf.Collections.Generic;
using Secyud.Ugf.LayoutComponents;
using UnityEngine;

namespace Secyud.Ugf.TreeViewComponents
{
    public class TreeNodeView:MonoBehaviour
    {
        [SerializeField] private TreeNodeView NodePrefab;
        [SerializeField] private LayoutGroupTrigger GroupPrefab;
        [SerializeField] private TreeNodeView ParentContent;

        private LayoutGroupTrigger Group { get; set; }
        private ITreeNode TreeNode { get; set; }

        public void Collapse(bool collapse)
        {
            if (Group)
                Destroy(Group.gameObject);
            TreeNode.Collapsed = collapse;
            if (collapse && !TreeNode.SubNodes.IsNullOrEmpty())
            {
                RectTransform rect = ParentContent.PrepareLayout(2);
                Group = GroupPrefab.Instantiate(rect);
                RectTransform subRect = Group.PrepareLayout();
                foreach (ITreeNode node in TreeNode.SubNodes)
                {
                    TreeNodeView nodeView = NodePrefab.Instantiate(subRect);
                    nodeView.TreeNode = node;
                    nodeView.ParentContent = this;
                    node.SetNode(nodeView);
                    if (node.Collapsed)
                        nodeView.Collapse(true);
                }
            }
        }

        private RectTransform PrepareLayout(int i)
        {
            if (ParentContent)
                 ParentContent.PrepareLayout(1+i);
            RectTransform ret = Group.PrepareLayout();
            Group.Record = i;
            return ret;
        }
    }
}
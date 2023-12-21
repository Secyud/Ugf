using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Ugf;
using System.Ugf.Collections.Generic;
using JetBrains.Annotations;

namespace Secyud.Ugf.VirtualPath
{
    public class PathNode
    {
        public string NodeName { get; }
        public PathNode ParentNode { get; }
        private SortedDictionary<string, PathNode> SubNodes { get; } = new();
        private SortedDictionary<string, List<Uri>> FileInfos { get; } = new();

        public PathNode(
            [NotNull] string nodeName,
            [CanBeNull] PathNode parentNode)
        {
            NodeName = nodeName;
            ParentNode = parentNode;
        }

        public void AddFile(string path)
        {
            string fileName = Path.GetFileName(path);
            List<Uri> filePaths = GetFiles(fileName);
            filePaths.AddIfNotContains(new Uri(path));
        }

        public void AddDirectory(string folderPath)
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                AddFile(file);
            }

            foreach (string subPath in Directory.GetDirectories(folderPath))
            {
                string subNodeName = Path.GetFileName(subPath);
                GetSubNode(subNodeName).AddDirectory(subPath);
            }
        }

        public string[] GetFilesInFolder()
        {
            List<string> ret = new();

            foreach (List<Uri> list in FileInfos.Values)
            {
                ret.AddRange(list.Select(u => u.LocalPath));
            }

            return ret.ToArray();
        }

        public string[] GetFilesSingly(string name)
        {
            return GetFiles(name).Select(u => u.LocalPath).ToArray();
        }

        private List<Uri> GetFiles(string name)
        {
            if (!FileInfos.TryGetValue(name, out List<Uri> files))
            {
                files = new List<Uri>();
                FileInfos[name] = files;
            }

            return files;
        }

        private PathNode GetSubNode(string name)
        {
            if (!SubNodes.TryGetValue(name, out PathNode descriptor))
            {
                descriptor = new(name, this);
                SubNodes[name] = descriptor;
            }

            return descriptor;
        }

        public virtual PathNode GetRelativeNode(string virtualPath)
        {
            string[] nodes = virtualPath.Split('/','\\');
            PathNode ret = this;
            foreach (string node in nodes)
            {
                string trimNode = node.Trim();
                if (trimNode.IsNullOrEmpty() ||
                    trimNode == ".")
                {
                }
                else if (trimNode == "..")
                {
                    ret = ret.ParentNode;
                }
                else
                {
                    ret = ret.GetSubNode(trimNode);
                }

                if (ret is null)
                {
                    U.LogError($"Invalid path: {virtualPath}");
                    return null;
                }
            }

            return ret;
        }
    }
}
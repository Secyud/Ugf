using System.IO;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.VirtualPath
{
    public class VirtualPathManager : IVirtualPathManager,IRegistry
    {
        private PathNode RootNode { get; } = new(string.Empty, null);

        public string[] GetFilesSingly(string virtualPath)
        {
            string path = Path.GetDirectoryName(virtualPath);
            string name = Path.GetFileName(virtualPath);
            return RootNode.GetRelativeNode(path).GetFilesSingly(name);
        }

        public string[] GetFilesInFolder(string virtualPath)
        {
            return RootNode.GetRelativeNode(virtualPath).GetFilesInFolder();
        }

        public void AddFile(string virtualPath, string filePath)
        {
            RootNode.GetRelativeNode(virtualPath).AddFile(filePath);
        }

        public void AddDirectory(string virtualPath, string folderPath)
        {
            RootNode.GetRelativeNode(virtualPath).AddDirectory(folderPath);
        }
    }
}
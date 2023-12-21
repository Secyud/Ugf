using System.Collections.Generic;

namespace Secyud.Ugf.VirtualPath
{
    public interface IVirtualPathManager
    {
        string[] GetFilesSingly(string virtualPath);
        string[] GetFilesInFolder(string virtualPath);
        void AddFile(string virtualPath, string filePath);
        void AddDirectory(string virtualPath, string folderPath);
    }
}
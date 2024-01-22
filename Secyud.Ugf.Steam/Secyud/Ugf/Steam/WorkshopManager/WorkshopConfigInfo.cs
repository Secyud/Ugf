using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class WorkshopConfigInfo
    {
        public ulong FieldId { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Assembly { get; set; }
        public bool Disabled { get; set; }
        public List<string> Tags { get; } = new();
        public Dictionary<string, string> AddTags { get; } = new();
        public List<string> RemoveTags { get; } = new();
        public string ChangeNote { get; set; }
        public List<string> MapFolders { get; } = new();
        public string AuthorName { get; set; }

        public bool ReadFromLocal(string localPath)
        {
            string path = Path.Combine(localPath, "info.json");
            if (File.Exists(path))
            {
                string jsonStr = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(jsonStr, this);
            }

            return false;
        }

        public void WriteToLocal(string localPath)
        {
            string path = Path.Combine(localPath, "info.json");
            string jsonStr = JsonUtility.ToJson(this);
            File.WriteAllText(path, jsonStr);
        }
    }
}
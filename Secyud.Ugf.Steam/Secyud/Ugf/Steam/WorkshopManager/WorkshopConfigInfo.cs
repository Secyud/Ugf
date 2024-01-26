using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
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
        public static WorkshopConfigInfo Default { get; } = new();

        public static WorkshopConfigInfo ReadFromLocal(string localPath)
        {
            string path = Path.Combine(localPath, "info.json");
            if (File.Exists(path))
            {
                string jsonStr = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<WorkshopConfigInfo>(jsonStr);
            }

            return null;
        }

        public static void WriteToLocal(WorkshopConfigInfo info,  string localPath)
        {
            string path = Path.Combine(localPath, "info.json");
            string jsonStr = JsonConvert.SerializeObject(info);
            File.WriteAllText(path, jsonStr);
        }
    }
}
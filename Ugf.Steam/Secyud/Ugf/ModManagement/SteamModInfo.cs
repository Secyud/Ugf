using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Secyud.Ugf.Modularity;
using Steamworks;
using UnityEngine;

namespace Secyud.Ugf.ModManagement
{
    public class SteamModInfo : IShowable, IObjectAccessor<Sprite>
    {
        public PublishedFileId_t Id { get; }

        public LocalInfo Local { get; private set; }

        public string Description => Local.Description;
        public string Name => Local.Name;
        public IObjectAccessor<Sprite> Icon => this;
        public Type ModuleType { get; private set; }
        public string Folder { get; private set; }

        private Sprite _value;

        public Sprite Value
        {
            get
            {
                if (!_value)
                {
                    var path = Path.Combine(Folder, "Icon.png");
                    if (File.Exists(path))
                    {
                        byte[] buffer = File.ReadAllBytes(path);
                        Texture2D tex = new(2, 2, TextureFormat.RGBA32, false);
                        try
                        {
                            tex.LoadImage(buffer);
                            _value = Sprite.Create(tex,
                                new Rect(0, 0, tex.width, tex.height),
                                new Vector2(0.5f, 0.5f));
                        }
                        catch (Exception e)
                        {
                            U.LogError(e);
                        }
                    }
                }

                return _value;
            }
        }

        public SteamModInfo(PublishedFileId_t id)
        {
            Id = id;
        }

        public void Refresh()
        {
            if (SteamUGC.GetItemInstallInfo(Id,
                    out ulong _, out string folder,
                    260, out uint _))
            {
                Local = LocalInfo.CreateFromContent(folder);

                if (Local is null)
                {
                    return;
                }

                Folder = folder;

                if (Local.Disabled)
                {
                    return;
                }

                string moduleAssemblyName = Path.Combine(folder, Local.ModuleAssemblyName);

                Assembly assembly = Assembly.LoadFrom(moduleAssemblyName);
                Type type = assembly.ExportedTypes.FirstOrDefault(
                    u => typeof(IUgfModule).IsAssignableFrom(u));

                if (type is not null)
                {
                    ModuleType = type;
                }
            }
        }

        public class LocalInfo : IHasName, IHasDescription
        {
            public Guid ModId { get; set; }
            public string Version { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ModuleAssemblyName { get; set; }
            public ulong FieldId { get; set; }
            public bool Disabled { get; set; }
            public byte Visibility { get; set; }
            public List<string> Tags { get; set; }
            public SortedDictionary<string,string> AddTags { get; set; }
            public List<string> RemoveTags { get; set; }
            public string ChangeNote { get; set; }

            public LocalInfo()
            {
            }

            public static LocalInfo CreateFromContent(string contentPath)
            {
                var path = Path.Combine(contentPath, "info.json");
                if (File.Exists(path))
                {
                    string jsonStr = File.ReadAllText(path);
                    LocalInfo info = JsonConvert.DeserializeObject<LocalInfo>(jsonStr);
                    return info;
                }

                return null;
            }

            public static void WriteToContent(LocalInfo info, string contentPath)
            {
                var path = Path.Combine(contentPath, "info.json");
                string jsonStr = JsonConvert.SerializeObject(info);
                File.WriteAllText(path, jsonStr);
            }

            public void WriteToContent(string fileName)
            {
                WriteToContent(this, fileName);
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Prefab
{
    public class PrefabManager : IPrefabManager, ISingleton
    {
        private readonly GameObject _canvas = GameObject.Find("Canvas");

        private readonly Dictionary<string, PrefabDescriptor> _uis = new();

        public void RegisterPrefabsInFolder(string path, bool isUi = false)
        {
            var absolutePath = Path.Combine(Application.dataPath, "Resources", path);
            
            var files = Directory
                .EnumerateFiles(absolutePath, "*.*",SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".prefab"))
                .Select(u=> Path.Combine(path, u[(absolutePath.Length+1)..u.LastIndexOf('.')]));

            RegisterPrefabs(files, isUi);
        }
    
        public void RegisterPrefabs(IEnumerable<string> prefabs, bool isUi = false)
        {
            foreach (var ui in prefabs)
                RegisterPrefab(ui, isUi);
        }

        public void RegisterPrefab(string path, bool isUi = false)
        {
            var descriptor = new PrefabDescriptor(path, CreateGameObject, isUi);
            _uis[descriptor.Name] = descriptor;
        }

        internal PrefabDescriptor GetDescriptor(string name)
        {
            return _uis[name];
        }

        private GameObject CreateGameObject(PrefabDescriptor descriptor, GameObject parent)
        {
            if (descriptor.IsUi && parent is null)
                parent = _canvas;

            var prefab = parent is null
                ? Object.Instantiate(
                    Resources.Load<GameObject>(descriptor.Path))
                : Object.Instantiate(
                    Resources.Load<GameObject>(descriptor.Path),
                    parent.transform);

            prefab.name = descriptor.Name;

            return prefab;
        }
    }
}
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Unity.Prefabs
{
    internal class PrefabDescriptor
    {
        public PrefabDescriptor(Type prefabType, bool isUi = true, string assetBundlePath = null)
        {
            UsingAssetBundle = assetBundlePath is not null;
            Path = UsingAssetBundle
                ? System.IO.Path.Combine(Application.dataPath, assetBundlePath!)
                : prefabType.FullName!.Replace('.', '/');
            Name = prefabType.Name;
            IsUi = isUi;
        }

        public bool UsingAssetBundle { get; }
        public bool IsUi { get; }
        
        public string Name { get; }

        public string Path { get; }

        public GameObject Create(GameObject parent, AssetBundle assetBundle = null)
        {
            var prefab = assetBundle is null
                ? Resources.Load<GameObject>(Path)
                : assetBundle.LoadAsset<GameObject>(Name);

            prefab = parent is null
                ? Object.Instantiate(prefab)
                : Object.Instantiate(prefab, parent.transform);

            prefab.name = Name;

            return prefab;
        }
    }
}
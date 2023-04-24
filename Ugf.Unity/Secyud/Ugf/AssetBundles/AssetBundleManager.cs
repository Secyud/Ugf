#region

using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetBundles
{
    [ExposeType(typeof(IAssetBundleProvider))]
    public class AssetBundleManager : IAssetBundleProvider, ISingleton
    {
        private readonly Dictionary<string, AssetBundle> _assetBundles = new();

        public AssetBundle GetByPath(string path)
        {
            if (_assetBundles.TryGetValue(path, out var bundle))
                return bundle;

            bundle = AssetBundle.LoadFromFile(path);
            _assetBundles[path] = bundle;
            return bundle;
        }

        public void ReleaseByPath(string path)
        {
            _assetBundles[path].Unload(false);

            _assetBundles[path] = null;
        }
    }
}
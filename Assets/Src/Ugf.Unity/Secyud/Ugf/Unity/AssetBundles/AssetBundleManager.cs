#region

using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.AssetBundles
{
    [ExposeType(typeof(IAssetBundleProvider))]
    public class AssetBundleManager : IAssetBundleProvider, ISingleton
    {
        private readonly Dictionary<string, AssetBundle> _assetBundles = new();

        public AssetBundle GetByPath(string path)
        {
            AssetBundle ret;

            if (_assetBundles.ContainsKey(path))
                ret = _assetBundles[path];
            else
            {
                ret = AssetBundle.LoadFromFile(path);
                _assetBundles[path] = ret;
            }

            return ret;
        }

        public void ReleaseByPath(string path)
        {
            _assetBundles[path].Unload(false);

            _assetBundles[path] = null;
        }
    }
}
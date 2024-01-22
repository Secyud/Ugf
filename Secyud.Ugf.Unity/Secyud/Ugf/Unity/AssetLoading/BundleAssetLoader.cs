using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Unity.AssetLoading
{
    public abstract class BundleAssetLoader : IAssetLoader
    {
        private readonly AssetBundle _bundle;

        protected BundleAssetLoader(string assetBundlePath)
        {
            _bundle = AssetBundle.LoadFromFile(assetBundlePath);
        }

        public virtual TAsset LoadAsset<TAsset>(string name)
            where TAsset : Object
        {
            return _bundle.LoadAsset<TAsset>(name);
        }

        public virtual void LoadAssetAsync<TAsset>(string name, Action<TAsset> useAction) where TAsset : Object
        {
            AssetBundleRequest request = _bundle.LoadAssetAsync(name);
            request.completed += _ => useAction?.Invoke(request.asset as TAsset);
        }

        public void Release<TAsset>(TAsset asset) where TAsset : Object
        {
        }
    }
}
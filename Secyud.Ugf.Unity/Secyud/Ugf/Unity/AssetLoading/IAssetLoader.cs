using System;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Unity.AssetLoading
{
    public interface IAssetLoader
    {
        public TAsset LoadAsset<TAsset>(string name)
            where TAsset : Object;

        public void LoadAssetAsync<TAsset>(string name,
            Action<TAsset> useAction)
            where TAsset : Object;

        public void Release<TAsset>(TAsset asset)
            where TAsset : Object;
    }
}
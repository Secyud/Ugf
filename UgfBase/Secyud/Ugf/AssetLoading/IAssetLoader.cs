using Secyud.Ugf.DependencyInjection;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.AssetLoading
{
    [Registry]
    public interface IAssetLoader
    {
        public TAsset LoadAsset<TAsset>(string name)
            where TAsset : Object;

        public void Release<TAsset>(TAsset asset)
            where TAsset : Object;
    }
}
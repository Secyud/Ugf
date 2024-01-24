using System;
using System.IO;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.Logging;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Unity.AssetLoading
{
    public class AssetContainer<TAsset> : ObjectContainer<TAsset, TAsset>, IArchivable, IDisposable
        where TAsset : Object
    {
        [S] protected IAssetLoader Loader;
        [S] protected string AssetName;

        protected AssetContainer()
        {
        }

        public static AssetContainer<TAsset> Create(
            IAssetLoader loader, string assetName)
        {
            return assetName.IsNullOrEmpty()
                ? null
                : new AssetContainer<TAsset>
                {
                    Loader = loader,
                    AssetName = assetName
                };
        }

        public static AssetContainer<TAsset> Create(
            Type loaderType, string assetName)
        {
            return Create(U.Get(loaderType) as IAssetLoader, assetName);
        }

        public static AssetContainer<TAsset> Create<TAssetLoader>(
            string assetName) where TAssetLoader : class, IAssetLoader
        {
            return Create(U.Get<TAssetLoader>(), assetName);
        }

        protected override TAsset HandleResult(TAsset result)
        {
            if (!result)
            {
                UgfLogger.LogError($"Failed get object {AssetName}.");
            }

            return result;
        }

        protected override TAsset GetOrigin()
        {
            return Loader.LoadAsset<TAsset>(AssetName);
        }

        protected override void GetOriginAsync(Action<TAsset> callback)
        {
            Loader.LoadAssetAsync(AssetName, callback);
        }

        public override TAsset GetValue()
        {
            if (!Instance)
                Instance = HandleResult(GetOrigin());
            return Instance;
        }

        public override void GetValueAsync(Action<TAsset> callback)
        {
            if (Instance)
            {
                callback.Invoke(Instance);
            }
            else
            {
                GetOriginAsync(o =>
                {
                    Instance = HandleResult(o);
                    callback.Invoke(Instance);
                });
            }
        }

        public override void Save(BinaryWriter writer)
        {
            writer.WriteNullable(Loader);
            writer.Write(AssetName);
        }

        public override void Load(BinaryReader reader)
        {
            Loader = reader.ReadNullable<IAssetLoader>();
            AssetName = reader.ReadString();
        }

        public void Dispose()
        {
            if (Instance)
            {
                Loader.Release(Instance);
            }

            Instance = null;
        }
    }
}
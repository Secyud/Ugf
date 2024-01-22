#region

using System;
using System.IO;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DataManager;
using Object = UnityEngine.Object;

#endregion

namespace Secyud.Ugf.Unity.AssetLoading
{
    public class AssetContainer<TAsset> : ObjectContainer<TAsset>, IArchivable,IDisposable
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
            return assetName.IsNullOrEmpty() ? null : new AssetContainer<TAsset>
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

        public override TAsset Value => CurrentInstance
            ? CurrentInstance
            : CurrentInstance = GetObject();

        protected override TAsset GetObject()
        {
            return Loader.LoadAsset<TAsset>(AssetName);
        }

        public virtual void Save(BinaryWriter writer)
        {
            writer.WriteNullable(Loader);
            writer.Write(AssetName);
        }

        public virtual void Load(BinaryReader reader)
        {
            Loader = reader.ReadNullable<IAssetLoader>();
            AssetName = reader.ReadString();
        }
        
        public void Dispose()
        {
            if (CurrentInstance)
            {
                Loader.Release(CurrentInstance);
            }
            CurrentInstance = null;
        }
    }
}
﻿#region

using Secyud.Ugf.Archiving;
using Secyud.Ugf.Container;
using System;
using System.Ugf;
using Secyud.Ugf.DataManager;
using Object = UnityEngine.Object;

#endregion

namespace Secyud.Ugf.AssetLoading
{
    public class AssetContainer<TAsset> : ObjectContainer<TAsset>, IArchivable
        where TAsset : Object
    {
        [S(ID = 0)] protected IAssetLoader Loader;
        [S(ID = 1)] protected string AssetName;

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

        ~AssetContainer()
        {
            Release();
        }

        public override void Release()
        {
            if (CurrentInstance)
                Loader.Release(CurrentInstance);
            CurrentInstance = null;
        }

        public virtual void Save(IArchiveWriter writer)
        {
            U.AutoSaveObject(this, writer);
        }

        public virtual void Load(IArchiveReader reader)
        {
            U.AutoLoadObject(this, reader);
        }
    }
}
using System;
using System.IO;
using System.Runtime.InteropServices;
using Secyud.Ugf.Unity.AssetLoading;
using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    [Guid("444be33a-104a-1d04-a043-0b7b00d798bc")]
    public class UgfUnitEffectContainer : PrefabContainer<UgfUnitEffect>
    {
        public new static PrefabContainer<UgfUnitEffect> Create(
            IAssetLoader loader, string assetName)
        {
            return assetName.IsNullOrEmpty()
                ? null
                : new UgfUnitEffectContainer
                {
                    Loader = loader,
                    AssetName = assetName
                };
        }

        public new static PrefabContainer<UgfUnitEffect> Create(
            Type loaderType, string assetName)
        {
            return Create(U.Get(loaderType) as IAssetLoader, assetName);
        }

        public new static PrefabContainer<UgfUnitEffect> Create<TAssetLoader>(
            string assetName) where TAssetLoader : class, IAssetLoader
        {
            return Create(U.Get<TAssetLoader>(), assetName);
        }

        protected override UgfUnitEffect GetObject()
        {
            return Loader
                .LoadAsset<GameObject>("UgfUnitEffect/" + AssetName + ".prefab")
                ?.GetComponent<UgfUnitEffect>();
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
    }
}
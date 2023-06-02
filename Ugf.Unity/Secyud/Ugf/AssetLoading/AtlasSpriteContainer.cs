#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Secyud.Ugf.Resource;
using UnityEngine;
using UnityEngine.U2D;

#endregion

namespace Secyud.Ugf.AssetLoading
{
    public class AtlasSpriteContainer : SpriteContainer
    {
        private SpriteAtlas _atlas;
        protected string AtlasName;

        protected AtlasSpriteContainer()
        {
        }

        protected AtlasSpriteContainer(
            [NotNull] IAssetLoader assetLoader,
            [NotNull] string atlasName,
            [NotNull] string assetName)
            : base(assetLoader, assetName)
        {
            AtlasName = $"Images/Atlas/{atlasName}.spriteatlas";
        }

        public static AtlasSpriteContainer Create(
            [NotNull] IAssetLoader assetLoader,
            [NotNull] string atlasName,
            string assetName)
        {
            return assetName.IsNullOrEmpty()
                ? null
                : new AtlasSpriteContainer(
                    assetLoader, atlasName, assetName
                );
        }

        public static AtlasSpriteContainer Create(
            [NotNull] Type type,
            [NotNull] string atlasName,
            [NotNull] string assetName)
        {
            return Create(Og.GetAssetLoader(type), atlasName, assetName);
        }

        public new static SpriteContainer Create(
            [NotNull] IAssetLoader loader,
            [NotNull] ResourceDescriptor descriptor,
            int id)
        {
            string atlasName = descriptor.Get<string>(id);
            string spriteName = descriptor.Get<string>(id + 1);

            return atlasName.IsNullOrEmpty()
                ? SpriteContainer.Create(loader, spriteName)
                : Create(loader, atlasName, spriteName);
        }

        protected override Sprite GetObject()
        {
            _atlas ??= Loader.LoadAsset<SpriteAtlas>(AtlasName);
            return _atlas ? _atlas.GetSprite(AssetName) : null;
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            writer.Write(AtlasName);
        }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);
            AtlasName = reader.ReadString();
        }

        public override void Release()
        {
            base.Release();
            _atlas = null;
        }
    }
}
#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Ugf;
using Secyud.Ugf.DataManager;
using UnityEngine;
using UnityEngine.U2D;

#endregion

namespace Secyud.Ugf.AssetLoading
{
    public class AtlasSpriteContainer : SpriteContainer
    {
        [S(ID = 2)] protected string AtlasName;
        private SpriteAtlas _atlas;

        protected AtlasSpriteContainer()
        {
        }

        public static AtlasSpriteContainer Create(
             IAssetLoader loader,
             string atlasName, string spriteName)
        {
            return spriteName.IsNullOrEmpty()
                ? null
                : new AtlasSpriteContainer()
                {
                    Loader = loader,
                    AtlasName = $"Images/Atlas/{atlasName}.spriteatlas",
                    AssetName = spriteName
                };
        }

        public static AtlasSpriteContainer Create(
             Type type,
             string atlasName, string spriteName)
        {
            return Create(U.Get(type) as IAssetLoader, atlasName, spriteName);
        }
        
        protected override Sprite GetObject()
        {
            _atlas ??= Loader.LoadAsset<SpriteAtlas>(AtlasName);
            return _atlas ? _atlas.GetSprite(AssetName) : null;
        }
        
        public override void Release()
        {
            base.Release();
            _atlas = null;
        }
    }
}
#region

using System;
using System.Ugf;
using Secyud.Ugf.AssetLoading;
using Secyud.Ugf.DataManager;
using UnityEngine;
using UnityEngine.U2D;

#endregion

namespace Secyud.Ugf.AssetComponents
{
    public class AtlasSpriteContainer : SpriteContainer
    {
        [S] protected string AtlasName;

        protected AtlasSpriteContainer()
        {
        }

        public static AtlasSpriteContainer Create(
             IAssetLoader loader,
             string atlasName, string spriteName)
        {
            return spriteName.IsNullOrEmpty()
                ? null
                : new AtlasSpriteContainer
                {
                    Loader = loader,
                    AtlasName = atlasName,
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
            return  Loader.LoadAsset<Sprite>($"Images/{AtlasName}/{AssetName}.png");
        }
    }
}
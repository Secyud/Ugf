﻿using System;
using System.Runtime.InteropServices;
using System.Ugf;
using JetBrains.Annotations;
using UnityEngine;

namespace Secyud.Ugf.Unity.AssetLoading
{
    // ReSharper disable InconsistentNaming
    public enum SpriteSuffix
    {
        png
    }

    [Guid("0D68AF8E-2D14-40BA-ADF6-75514A599FCE")]
    public class SpriteContainer : AssetContainer<Sprite>
    {
        protected SpriteContainer()
        {
        }

        public static SpriteContainer Create(
            IAssetLoader loader,
            string spriteName,
            SpriteSuffix suffix = SpriteSuffix.png)
        {
            return spriteName.IsNullOrEmpty()
                ? null
                : new SpriteContainer
                {
                    AssetLoader = loader,
                    AssetName = $"{spriteName}.{suffix}"
                };
        }

        public static SpriteContainer Create(
            [NotNull] Type type,
            [NotNull] string spriteName,
            SpriteSuffix suffix = SpriteSuffix.png)
        {
            return Create(U.Get(type) as IAssetLoader, spriteName, suffix);
        }

        public static SpriteContainer Create<TAssetLoader>(
            [NotNull] string spriteName,
            SpriteSuffix suffix = SpriteSuffix.png)
            where TAssetLoader : class, IAssetLoader
        {
            return Create(U.Get<TAssetLoader>(), spriteName, suffix);
        }
    }
}
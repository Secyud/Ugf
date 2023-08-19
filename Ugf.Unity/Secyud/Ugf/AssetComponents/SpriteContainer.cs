#region

using System;
using System.Ugf;
using JetBrains.Annotations;
using Secyud.Ugf.AssetLoading;
using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetComponents
{
	// ReSharper disable InconsistentNaming
	public enum SpriteSuffix
	{
		png
	}

	// ReSharper restore InconsistentNaming

	public enum SpritePrefix
	{
		Icons, Art
	}

	public class SpriteContainer : AssetContainer<Sprite>
	{
		protected SpriteContainer()
		{
		}

		public static SpriteContainer Create(
			IAssetLoader loader,
			string spriteName,
			SpritePrefix prefix = SpritePrefix.Icons,
			SpriteSuffix suffix = SpriteSuffix.png)
		{
			return spriteName.IsNullOrEmpty()
				? null
				: new SpriteContainer
				{
					Loader = loader,
					AssetName = $"Images/{prefix}/{spriteName}.{suffix}"
				};
		}

		public static SpriteContainer Create(
			[NotNull] Type type,
			[NotNull] string spriteName,
			SpritePrefix prefix = SpritePrefix.Icons,
			SpriteSuffix suffix = SpriteSuffix.png)
		{
			return Create(U.Get(type) as IAssetLoader, spriteName, prefix, suffix);
		}

		public static SpriteContainer Create<TAssetLoader>(
			[NotNull] string spriteName,
			SpritePrefix prefix = SpritePrefix.Icons,
			SpriteSuffix suffix = SpriteSuffix.png)
			where TAssetLoader : class, IAssetLoader
		{
			return Create(U.Get<TAssetLoader>(), spriteName, prefix, suffix);
		}
	}
}
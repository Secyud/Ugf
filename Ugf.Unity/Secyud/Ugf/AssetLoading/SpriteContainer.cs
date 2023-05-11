#region

using JetBrains.Annotations;
using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetLoading
{
	// ReSharper disable InconsistentNaming
	public enum SpriteSuffix
	{
		png
	}

	// ReSharper restore InconsistentNaming
	public enum SpritePrefix
	{
		Icons
	}

	public class SpriteContainer : AssetContainer<Sprite>
	{
		protected SpriteContainer()
		{
		}

		protected SpriteContainer(
			[NotNull] IAssetLoader loader,
			[NotNull] string assetName)
			: base(loader,  assetName)
		{
		}

		public static SpriteContainer Create(
			[NotNull] IAssetLoader container,
			[NotNull] string assetName,
			SpritePrefix prefix = SpritePrefix.Icons,
			SpriteSuffix suffix = SpriteSuffix.png)
		{
			return new SpriteContainer(container, $"Images/{prefix}/{assetName}.{suffix}");
		}

		public static SpriteContainer Create(
			[NotNull] Type type,
			[NotNull] string assetName,
			SpritePrefix prefix = SpritePrefix.Icons,
			SpriteSuffix suffix = SpriteSuffix.png)
		{
			return Create(Og.GetAssetLoader(type), assetName,prefix, suffix);
		}

		public static SpriteContainer Create<TAbBase>(
			[NotNull] string spriteName,
			SpritePrefix prefix = SpritePrefix.Icons,
			SpriteSuffix suffix = SpriteSuffix.png)
			where TAbBase : IAssetLoader
		{
			return Create(typeof(TAbBase), spriteName,prefix, suffix);
		}
	}
}
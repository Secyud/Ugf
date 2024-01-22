#region

using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.AssetLoading
{
	// ReSharper disable InconsistentNaming
	public enum SpriteSuffix
	{
		png
	}

	[Guid("e98b3736-5cc4-31bb-0fd7-14bc3dc329cf")]
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
					Loader = loader,
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
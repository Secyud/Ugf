#region

using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetBundles
{
	public class BundleSpriteContainer : BundleAssetContainer<Sprite>
	{
		protected BundleSpriteContainer()
		{
		}

		public BundleSpriteContainer(Type assetBundleType, string assetName)
			: base(assetBundleType, assetName)
		{
		}

		public BundleSpriteContainer(AssetBundleContainer assetBundleContainer, string assetName)
			: base(assetBundleContainer, assetName)
		{
		}

		public static BundleSpriteContainer Icon<TAbBase>(string spriteName)
			where TAbBase : AssetBundleBase
		{
			return Icon(typeof(TAbBase), spriteName);
		}

		public static BundleSpriteContainer Icon(Type abType, string spriteName)
		{
			return new BundleSpriteContainer(abType, spriteName + "_64_64");
		}

		public new static BundleSpriteContainer Create<TAbBase>(string spriteName)
			where TAbBase : AssetBundleBase
		{
			return new BundleSpriteContainer(typeof(TAbBase), spriteName);
		}
	}
}
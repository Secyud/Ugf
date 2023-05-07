using Secyud.Ugf.AssetBundles;
using System;
using UnityEngine;

namespace Secyud.Ugf.Container
{
	public class SpriteContainer : AssetContainer<Sprite>
	{
		protected SpriteContainer()
		{
		}

		public SpriteContainer(Type assetBundleType, string assetName)
			: base(assetBundleType, assetName)
		{
		}

		public SpriteContainer(AssetBundleContainer assetBundleContainer, string assetName)
			: base(assetBundleContainer, assetName)
		{
		}

		public static SpriteContainer Icon<TAbBase>(string spriteName)
			where TAbBase : AssetBundleBase
		{
			return Icon(typeof(TAbBase), spriteName);
		}

		public static SpriteContainer Icon(Type abType, string spriteName)
		{
			return  new SpriteContainer(abType, spriteName + "_64_64");
		}

		public new static SpriteContainer Create<TAbBase>(string spriteName)
			where TAbBase : AssetBundleBase
		{
			return new SpriteContainer(typeof(TAbBase), spriteName);
		}
	}
}
using Secyud.Ugf.AssetBundles;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

namespace Secyud.Ugf.Container
{
	public class SpriteFromAtlasContainer : SpriteContainer
	{
		private SpriteAtlas _atlas; 
		protected string AtlasName;
		
		protected SpriteFromAtlasContainer()
		{
			
		}

		public SpriteFromAtlasContainer(Type assetBundleType, string atlasName, string assetName)
			: base(assetBundleType, assetName)
		{
			AtlasName = atlasName;
		}

		public SpriteFromAtlasContainer(AssetBundleContainer assetBundleContainer, string atlasName,
			string assetName)
			: base(assetBundleContainer, assetName)
		{
			AtlasName = atlasName;
		}

		protected override Sprite GetObject()
		{
			_atlas ??= AssetBundleContainer.Value.LoadAsset<SpriteAtlas>(AtlasName);
			return _atlas.GetSprite(AssetName);
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
		
		public static SpriteFromAtlasContainer Icon<TAbBase>(string atlasName,string spriteName)
			where TAbBase: AssetBundleBase
		{
			return Icon(typeof(TAbBase),atlasName,spriteName);
		}
		public static SpriteFromAtlasContainer Icon(Type abType,string atlasName,string spriteName)
		{
			return new SpriteFromAtlasContainer(abType, atlasName, spriteName + "_64_64");
		}
	}
}
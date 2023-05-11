#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
			[NotNull] string assetName)
		{
			return new AtlasSpriteContainer(
				assetLoader,
				atlasName, assetName
			);
		}

		public static AtlasSpriteContainer Create(
			[NotNull] Type type,
			[NotNull] string atlasName,
			[NotNull] string assetName)
		{
			return Create(Og.GetAssetLoader(type), atlasName, assetName);
		}

		protected override Sprite GetObject()
		{
			_atlas ??= Loader.LoadAsset<SpriteAtlas>(AtlasName);
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

		public override void Release()
		{
			base.Release();
			_atlas = null;
		}
	}
}
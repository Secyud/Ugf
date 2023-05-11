#region

using JetBrains.Annotations;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.Container;
using System;
using System.IO;
using Object = UnityEngine.Object;

#endregion

namespace Secyud.Ugf.AssetLoading
{
	public class AssetContainer<TAsset> : ObjectContainer<TAsset>, IArchivable
		where TAsset : Object
	{
		protected IAssetLoader Loader;
		protected string AssetName;

		protected AssetContainer()
		{
		}

		
		protected AssetContainer(
			[NotNull] IAssetLoader loader,
			[NotNull] string assetName)
		{
			Loader = loader;
			AssetName = assetName;
		}

		public static AssetContainer<TAsset> Create(
			[NotNull] IAssetLoader container,
			[NotNull] string assetName)
		{
			return new AssetContainer<TAsset>(container, assetName);
		}

		public static AssetContainer<TAsset> Create(
			[NotNull] Type loaderType,
			[NotNull] string assetName)
		{
			return Create(
				Og.GetAssetLoader(loaderType),
				assetName
			);
		}

		public static AssetContainer<TAsset> Create<TAbBase>(
			[NotNull] string assetName)
			where TAbBase : IAssetLoader
		{
			return Create(typeof(TAbBase), assetName);
		}

		public override TAsset Value => CurrentInstance
			? CurrentInstance : CurrentInstance = GetObject();

		protected override TAsset GetObject()
		{
			return Loader.LoadAsset<TAsset>(AssetName);
		}

		~AssetContainer()
		{
			Release();
		}
		public override void Release()
		{
			if (CurrentInstance)
				Loader.Release(CurrentInstance);
			CurrentInstance = null;
		}

		public virtual void Save(BinaryWriter writer)
		{
			writer.Write(Loader.GetTypeId());
			writer.Write(AssetName);
		}

		public virtual void Load(BinaryReader reader)
		{
			Loader = Og.Provider.Get(
				Og.TypeManager[reader.ReadGuid()].Type
			) as IAssetLoader;
			AssetName = reader.ReadString();
		}
	}
}
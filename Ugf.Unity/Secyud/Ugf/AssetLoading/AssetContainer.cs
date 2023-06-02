#region

using JetBrains.Annotations;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.Container;
using System;
using System.IO;
using Secyud.Ugf.Resource;
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
			 string assetName)
		{
			return assetName.IsNullOrEmpty()? null: new AssetContainer<TAsset>(container, assetName);
		}

		public static AssetContainer<TAsset> Create(
			 Type loaderType,
			 string assetName)
		{
			return Create(
				Og.GetAssetLoader(loaderType),
				assetName
			);
		}

		public static AssetContainer<TAsset> Create<TAbBase>(
			 string assetName)
			where TAbBase : IAssetLoader
		{
			return Create(typeof(TAbBase), assetName);
		}
		
		public static AssetContainer<TAsset> Create(
			[NotNull] IAssetLoader loader,
			[NotNull] ResourceDescriptor descriptor,
			int id)
		{
			string assetName = descriptor.Get<string>(id);

			return Create(loader,assetName);
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
			Loader = Og.DefaultProvider.Get(
				Og.ClassManager[reader.ReadGuid()].Type
			) as IAssetLoader;
			AssetName = reader.ReadString();
		}
	}
}
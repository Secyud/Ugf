using JetBrains.Annotations;
using Secyud.Ugf.AssetBundles;
using System;
using System.IO;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Container
{
	public class AssetContainer<TAsset> : ObjectContainer<TAsset>
		where TAsset : Object
	{
		protected AssetBundleContainer AssetBundleContainer;
		protected string AssetName;

		public AssetContainer()
		{
		}

		public AssetContainer([NotNull] AssetBundleContainer assetBundleContainer, string assetName)
		{
			AssetBundleContainer = assetBundleContainer;
			AssetName = assetName;
		}

		public AssetContainer(Type assetBundleContainerType, string assetName)
		{
			AssetBundleContainer = Og.Provider.Get(assetBundleContainerType) as AssetBundleContainer;
			AssetName = assetName;
		}

		public override TAsset Value => CurrentInstance ? CurrentInstance : CurrentInstance = GetObject();

		protected override TAsset GetObject()
		{
			return AssetBundleContainer.Value.LoadAsset<TAsset>(AssetName);
		}

		public virtual void Save(BinaryWriter writer)
		{
			if (AssetBundleContainer is AssetBundleBase)
			{
				writer.Write(true);
				writer.Write(AssetBundleContainer.GetTypeId());
			}
			else
			{
				writer.Write(false);
				writer.Write(AssetBundleContainer.AssetBundleName);
			}
			writer.Write(AssetName);
		}

		public virtual void Load(BinaryReader reader)
		{
			if (reader.ReadBoolean())
				AssetBundleContainer = Og.Provider.Get(
					Og.TypeManager[reader.ReadGuid()].Type
				) as AssetBundleContainer;
			else
				AssetBundleContainer = new AssetBundleContainer(reader.ReadString());
			AssetName = reader.ReadString();
		}

		public static AssetContainer<TAsset> Create<TAbBase>(string spriteName)
			where TAbBase : AssetBundleBase
		{
			return Create(typeof(TAbBase), spriteName);
		}

		public static AssetContainer<TAsset> Create(Type abType, string assetName)
		{
			return new AssetContainer<TAsset>(abType, assetName);
		}
	}
}
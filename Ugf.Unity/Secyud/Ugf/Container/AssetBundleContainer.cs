using Secyud.Ugf.Archiving;
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Container
{
	public class AssetBundleContainer : ObjectContainer<AssetBundle>, IArchivable
	{
		protected string AssetBundleName;

		public AssetBundleContainer()
		{
		}

		public AssetBundleContainer(string assetBundleName)
		{
			AssetBundleName = assetBundleName;
		}

		protected override AssetBundle GetObject()
		{
			return Og.GetAssetBundle(AssetBundleName);
		}

		public override AssetBundle Value => Instance ? Instance : Instance ??= GetObject();

		public void Save(BinaryWriter writer)
		{
			writer.Write(AssetBundleName);
		}

		public void Load(BinaryReader reader)
		{
			AssetBundleName = reader.ReadString();
		}

		public TAsset LoadObject<TAsset>(string name)
			where TAsset : Component
		{
			GameObject obj = LoadAsset<GameObject>(name);

			if (obj is null)
				throw new NullReferenceException($"Cannot find game object named {name} in {AssetBundleName}");

			return obj.GetComponent<TAsset>();
		}

		public TAsset LoadAsset<TAsset>(string name)
			where TAsset : Object
		{
			return Value ? Value.LoadAsset<TAsset>(name) : null;
		}
	}
}
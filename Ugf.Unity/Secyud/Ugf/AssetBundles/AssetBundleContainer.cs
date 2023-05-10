#region

using Secyud.Ugf.Container;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Secyud.Ugf.AssetBundles
{
	public class AssetBundleContainer : ObjectContainer<AssetBundle>
	{
		public readonly string AssetBundleName;

		protected AssetBundleContainer()
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

		public override AssetBundle Value => CurrentInstance ? CurrentInstance : CurrentInstance ??= GetObject();

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
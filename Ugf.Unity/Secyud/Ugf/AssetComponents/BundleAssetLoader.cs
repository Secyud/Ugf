using System;
using Secyud.Ugf.AssetLoading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.AssetComponents
{
	public abstract class BundleAssetLoader : IAssetLoader
	{
		private readonly AssetBundle _bundle;

		protected BundleAssetLoader(string assetBundlePath)
		{
			_bundle = AssetBundle.LoadFromFile(assetBundlePath);
		}


		public virtual TComponent LoadObject<TComponent>(string name)
			where TComponent : Component
		{
			GameObject obj = LoadAsset<GameObject>(name);

			if (obj is null)
				throw new NullReferenceException($"Cannot find game object named {name} in {_bundle}");

			return obj.GetComponent<TComponent>();
		}

		public void Release<TAsset>(TAsset asset) where TAsset : Object
		{
			
		}

		public virtual TAsset LoadAsset<TAsset>(string name)
			where TAsset : Object
		{
			return _bundle.LoadAsset<TAsset>(name);
		}
	}
}
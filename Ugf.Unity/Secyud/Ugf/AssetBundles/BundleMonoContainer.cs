#region

using Secyud.Ugf.Container;
using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetBundles
{
	public class BundleMonoContainer<TComponent> : BundleAssetContainer<TComponent>, IMonoContainer<TComponent>
		where TComponent : MonoBehaviour
	{
		private readonly bool _onCanvas;
		private TComponent _prefab;

		protected BundleMonoContainer()
		{
		}

		public BundleMonoContainer(AssetBundleContainer assetBundleBase, string name, bool onCanvas = true)
			: base(assetBundleBase, name)
		{
			_onCanvas = onCanvas;
			_prefab = null;
		}

		public BundleMonoContainer(Type assetBundleType, string name, bool onCanvas = true)
			: base(assetBundleType, name)
		{
			_onCanvas = onCanvas;
			_prefab = null;
		}

		public static BundleMonoContainer<TComponent> Create<TAbBase>(string name, bool onCanvas = true)
			where TAbBase : AssetBundleBase
		{
			return new BundleMonoContainer<TComponent>(typeof(TAbBase), name, onCanvas);
		}

		protected override TComponent GetObject()
		{
			return AssetBundleContainer.LoadObject<TComponent>(AssetName);
		}

		public override TComponent Value => CurrentInstance;

		public TComponent Create()
		{
			if (!_prefab)
			{
				_prefab = GetObject();
				if (!_prefab)
					Debug.LogError($"{typeof(TComponent)} cannot be found!");
			}

			if (CurrentInstance)
				CurrentInstance.Destroy();

			CurrentInstance = _onCanvas ? _prefab.InstantiateOnCanvas() : _prefab.Instantiate();

			return CurrentInstance;
		}

		public void Destroy()
		{
			if (CurrentInstance) CurrentInstance.Destroy();
			CurrentInstance = null;
		}
	}
}
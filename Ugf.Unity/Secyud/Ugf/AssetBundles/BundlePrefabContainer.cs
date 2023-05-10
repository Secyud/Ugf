#region

using JetBrains.Annotations;
using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetBundles
{
	public class BundlePrefabContainer<TComponent> : BundleAssetContainer<TComponent>
		where TComponent : Component
	{
		public BundlePrefabContainer([NotNull] AssetBundleContainer assetBundleContainer, string assetName)
			: base(assetBundleContainer, assetName)
		{
		}

		public BundlePrefabContainer(Type assetBundleContainerType, string assetName)
			: base(assetBundleContainerType, assetName)
		{
		}

		protected BundlePrefabContainer()
		{
		}

		protected override TComponent GetObject()
		{
			var obj = AssetBundleContainer.Value.LoadAsset<GameObject>(AssetName);
			return obj ? obj.GetComponent<TComponent>() : null;
		}

		public TComponent Instantiate()
		{
			return Value.Instantiate();
		}

		public TComponent Instantiate(Transform parent)
		{
			return Value.Instantiate(parent);
		}
	}
}
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Secyud.Ugf.Container
{
	public class PrefabContainer<TComponent> : AssetContainer<TComponent>
		where TComponent : Component
	{

		public PrefabContainer([NotNull] AssetBundleContainer assetBundleContainer, string assetName)
			: base(assetBundleContainer, assetName)
		{
		}

		public PrefabContainer(Type assetBundleContainerType, string assetName)
			: base(assetBundleContainerType, assetName)
		{
		}
		
		public PrefabContainer()
		{
		}

		protected override TComponent GetObject()
		{
			var obj = AssetBundleContainer.Value.LoadAsset<GameObject>(AssetName);
			return obj ?  obj.GetComponent<TComponent>():null;
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
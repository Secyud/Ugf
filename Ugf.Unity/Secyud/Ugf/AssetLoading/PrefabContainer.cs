#region

using InfinityWorldChess.GlobalDomain;
using JetBrains.Annotations;
using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetLoading
{
	public class PrefabContainer<TComponent> : AssetContainer<TComponent>
		where TComponent : Component
	{
		protected PrefabContainer()
		{
		}

		protected PrefabContainer(
			[NotNull] IAssetLoader loader,
			[CanBeNull] string assetName = null)
			: base(loader, assetName ?? Og.TypeToPath<TComponent>()+".prefab")
		{
		}

		public new static PrefabContainer<TComponent> Create(
			[NotNull] IAssetLoader container,
			[CanBeNull] string assetName = null)
		{
			return new PrefabContainer<TComponent>(container, assetName);
		}

		public new static PrefabContainer<TComponent> Create(
			[NotNull] Type loaderType, 
			[CanBeNull] string assetName = null)
		{
			return Create(Og.GetAssetLoader(loaderType), assetName);
		}

		protected override TComponent GetObject()
		{
			var obj = Loader.LoadAsset<GameObject>(AssetName);
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
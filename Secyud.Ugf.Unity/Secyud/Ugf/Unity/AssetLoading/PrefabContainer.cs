#region

using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.AssetLoading
{
	public class PrefabContainer<TComponent> : AssetContainer<TComponent>
		where TComponent : Component
	{
		protected PrefabContainer()
		{
		}
		
		public new static PrefabContainer<TComponent> Create(
			IAssetLoader loader,
			string prefabName = null)
		{
			return new PrefabContainer<TComponent>
			{
				Loader = loader,
				AssetName = prefabName ?? U.TypeToPath<TComponent>() + ".prefab"
			};
		}

		public new static PrefabContainer<TComponent> Create(
			Type loaderType,
			string prefabName = null)
		{
			return Create(U.Get(loaderType) as IAssetLoader, prefabName);
		}

		public new static PrefabContainer<TComponent> Create<TAssetLoader>(string prefabName = null)
			where TAssetLoader : class, IAssetLoader
		{
			return Create(U.Get<TAssetLoader>(), prefabName);
		}
		
		protected override TComponent GetObject()
		{
			GameObject obj = Loader.LoadAsset<GameObject>(AssetName);
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
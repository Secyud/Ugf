#region

using JetBrains.Annotations;
using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetLoading
{
	public class MonoContainer<TComponent> : AssetContainer<TComponent>, IMonoContainer<TComponent>
		where TComponent : MonoBehaviour
	{
		private readonly bool _onCanvas;
		private TComponent _prefab;

		protected MonoContainer()
		{
		}

		protected MonoContainer(
			[NotNull] IAssetLoader loader,
			[CanBeNull] string name = null,
			bool onCanvas = true)
			: base(loader, name ?? Og.TypeToPath<TComponent>()+".prefab")
		{
			_onCanvas = onCanvas;
			_prefab = null;
		}

		public static MonoContainer<TComponent> Create(
			[NotNull] IAssetLoader container,
			[CanBeNull] string name = null,
			bool onCanvas = true)
		{
			return new MonoContainer<TComponent>(container, name, onCanvas);
		}

		public static MonoContainer<TComponent> Create(
			[NotNull] Type loaderType,
			[CanBeNull] string name = null,
			bool onCanvas = true)
		{
			return Create(Og.GetAssetLoader(loaderType), name, onCanvas);
		}

		public static MonoContainer<TComponent> Create<TAbBase>(
			[CanBeNull] string name = null,
			bool onCanvas = true)
			where TAbBase : IAssetLoader
		{
			return Create(typeof(TAbBase), name, onCanvas);
		}

		public static MonoContainer<TComponent> Create<TAbBase>(bool onCanvas = true)
			where TAbBase : IAssetLoader
		{
			return Create<TAbBase>(null, onCanvas);
		}


		protected override TComponent GetObject()
		{
			GameObject obj = Loader.LoadAsset<GameObject>(AssetName);
			return obj ? obj.GetComponent<TComponent>() : null;
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
#region

using Secyud.Ugf.AssetBundles;
using System;
using System.IO;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Container
{
	public class MonoContainer<TComponent> : AssetContainer<TComponent> 
		where TComponent : MonoBehaviour
	{
		private readonly bool _onCanvas;
		private TComponent _prefab;

		public MonoContainer(AssetBundleContainer assetBundleBase, string name, bool onCanvas = true)
			: base(assetBundleBase, name)
		{
			_onCanvas = onCanvas;
			_prefab = null;
		}

		public MonoContainer(Type assetBundleType, string name, bool onCanvas = true)
			: base(assetBundleType, name)
		{
			_onCanvas = onCanvas;
			_prefab = null;
		}

		public static MonoContainer<TComponent> Create<TAbBase>(string name, bool onCanvas = true)
			where TAbBase : AssetBundleBase
		{
			return new MonoContainer<TComponent>(typeof(TAbBase), name, onCanvas);
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
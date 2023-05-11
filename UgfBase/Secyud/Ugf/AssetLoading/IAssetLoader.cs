﻿using Secyud.Ugf.DependencyInjection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.AssetLoading
{
	public interface IAssetLoader: ISingleton
	{
		public TAsset LoadAsset<TAsset>(string name)
			where TAsset : Object;
		
		public void Release<TAsset>(TAsset asset)
			where TAsset : Object;
	}
}
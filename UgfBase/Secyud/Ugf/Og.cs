#region

using Localization;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.AssetLoading;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace Secyud.Ugf
{
	public static class Og
	{
		internal const BindingFlags ConstructFlag =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public static readonly Camera MainCamera = Camera.main;
		public static readonly Canvas Canvas = GameObject.Find("StaticObject/Canvas").GetComponent<Canvas>();
		public static readonly string DataPath = Application.dataPath;
		public static readonly string StreamingAssetPath = Path.Combine(DataPath, "StreamingAssets");

#if UNITY_EDITOR
		public static readonly string MainPath = DataPath[..^6];
#endif
		public static readonly string AssetBundlePath =
#if UNITY_EDITOR
			Path.Combine(MainPath, "Library/com.unity.addressables/aa/Windows/StandaloneWindows");
#else
			Path.Combine(StreamingAssetPath, "aa/StandaloneWindows/");
#endif
		public static string GetAssetBundlePath(string name)
		{
			return Path.Combine(AssetBundlePath, name);
		}


		public static IDependencyProvider Provider { get; private set; }

		public static IStringLocalizer<DefaultResource> L { get; private set; }

		public static ISpriteLocalizer<DefaultResource> IL { get; private set; }

		public static LoadingService LoadingService { get; private set; }

		public static TypeManager TypeManager { get; private set; }

		public static IAssetLoader GetAssetLoader(Type assetLoaderType)
		{
			return Provider.Get(assetLoaderType) as IAssetLoader;
		}

		internal static void Initialize(IDependencyProvider provider)
		{
			Provider = provider;
			L = Get<IStringLocalizer<DefaultResource>>();
			IL = Get<ISpriteLocalizer<DefaultResource>>();
			LoadingService = Get<LoadingService>();
			TypeManager = Get<TypeManager>();
		}

		public static T Get<T>() where T : class
		{
			return Provider.Get<T>();
		}

		public static int GetRandom(int max, int min = 0)
		{
			return Random.Range(min, max);
		}

		public static string TypeToPath<TObj>()
		{
			return TypeToPath(typeof(TObj));
		}

		public static string TypeToPath(Type type)
		{
			return type.FullName!.Replace('.', '/');
		}
	}
}
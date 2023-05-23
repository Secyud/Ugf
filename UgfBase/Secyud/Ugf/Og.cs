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
		public static readonly string StreamingAssetPath = Application.streamingAssetsPath;

#if UNITY_EDITOR
		public static readonly string AppPath = DataPath;
#else
		public static readonly string AppPath = Application.dataPath[..Application.dataPath.LastIndexOf("/", StringComparison.Ordinal)];
#endif

		public static readonly string ArchivingPath = Path.Combine(AppPath, "Archiving");

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
			return DotToPath(type.FullName);
		}
		public static string DotToPath(string name)
		{
			return name.Replace('.', '/');
		}
	}
}
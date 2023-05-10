#region

using Localization;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.AssetBundles;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;
using System.IO;
using System.Reflection;
using UnityEngine;

#endregion

namespace Secyud.Ugf
{
	public static class Og
	{
		internal const BindingFlags ConstructFlag =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public static readonly Camera MainCamera = Camera.main;
		public static readonly Canvas Canvas = GameObject.Find("StaticObject/Canvas").GetComponent<Canvas>();

		public static readonly string AppPath = Application.dataPath;

		public const string AssetBundlePath = "AssetBundles/StandaloneWindows/";

		public static string GetAssetBundlePath(string name)
		{
			return Path.Combine(AppPath, AssetBundlePath, name);
		}

		public static AssetBundle GetAssetBundle(string name)
		{
			return AssetBundleProvider.GetByPath(GetAssetBundlePath(name));
		}

		public static IDependencyProvider Provider { get; private set; }

		public static IStringLocalizer<DefaultResource> L { get; private set; }

		public static ISpriteLocalizer<DefaultResource> IL { get; private set; }

		public static LoadingService LoadingService { get; private set; }

		public static TypeManager TypeManager { get; private set; }

		public static IAssetBundleProvider AssetBundleProvider { get; private set; }

		internal static void Initialize(IDependencyProvider provider)
		{
			Provider = provider;
			L = Get<IStringLocalizer<DefaultResource>>();
			IL = Get<ISpriteLocalizer<DefaultResource>>();
			LoadingService = Get<LoadingService>();
			TypeManager = Get<TypeManager>();
			AssetBundleProvider = Get<IAssetBundleProvider>();
		}

		public static T Get<T>() where T : class
		{
			return Provider.Get<T>();
		}

		public static int GetRandom(int max, int min = 0)
		{
			return Random.Range(min, max);
		}
	}
}
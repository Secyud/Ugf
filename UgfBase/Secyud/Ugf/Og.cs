#region

using Localization;
using Secyud.Ugf.AssetLoading;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.Resource;
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
        public static readonly string AppPath = DataPath[..^6];
#else
		public static readonly string AppPath =
 Application.dataPath[..Application.dataPath.LastIndexOf("/", StringComparison.Ordinal)];
#endif

        public static readonly string ArchivingPath = Path.Combine(AppPath, "Archiving");

        public static IDependencyScopeFactory ScopeFactory { get; private set; }

        public static IStringLocalizer<DefaultResource> L { get; private set; }

        public static ISpriteLocalizer<DefaultResource> IL { get; private set; }

        public static LoadingService LoadingService { get; private set; }

        public static ClassManager ClassManager { get; private set; }
        public static InitializeManager InitializeManager { get; private set; }

        public static IAssetLoader GetAssetLoader(Type assetLoaderType)
        {
            return DefaultProvider.Get(assetLoaderType) as IAssetLoader;
        }

        public static IDependencyProvider DefaultProvider;

        internal static void Initialize(IDependencyProvider provider)
        {
            DefaultProvider = provider;
            ScopeFactory = provider.Get<IDependencyScopeFactory>();

            L = DefaultProvider.Get<IStringLocalizer<DefaultResource>>();
            IL = DefaultProvider.Get<ISpriteLocalizer<DefaultResource>>();
            LoadingService = DefaultProvider.Get<LoadingService>();
            ClassManager = DefaultProvider.Get<ClassManager>();
            InitializeManager = DefaultProvider.Get<InitializeManager>();
        }

        public static T Get<TScope, T>() where T : class where TScope : DependencyScope
        {
            return ScopeFactory.GetScope<TScope>().DependencyProvider.Get<T>();
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

        public static TTemplate ConstructTemplate<TTemplate>(string name)
            where TTemplate : ResourcedBase
        {
            Guid typeId = InitializeManager.GetResource(name).TypeId;
            if (typeId == default)
                typeId = typeof(TTemplate).GetTypeId();
            return ClassManager.Construct(typeId) as TTemplate;
        }

        public static TTemplate ConstructTemplateAndInit<TTemplate>(string name)
            where TTemplate : ResourcedBase
        {
            return ConstructTemplate<TTemplate>(name).Init(name);
        }

        public static IEnumerable<TTemplate> ConstructPrefabTemplates<TTemplate>(IEnumerable<string> names)
            where TTemplate : ResourcedBase
        {
            return names.Select(ConstructTemplateAndInit<TTemplate>);
        }
    }
}
#region

using Secyud.Ugf.Archiving;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.InputManaging;
using Secyud.Ugf.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

#endregion

namespace Secyud.Ugf.Modularity
{
	public class UgfApplication : IUgfApplication
	{
		private readonly IDependencyManager _dependencyManager;

		private bool _configuredServices;

		internal UgfApplication(
			IDependencyManager dependencyManager,
			Type startupModuleType,
			PlugInSourceList plugInSources = null)
		{
			Thrower.IfNull(dependencyManager);
			Thrower.IfNull(startupModuleType);

			StartupModuleType = startupModuleType;

			_dependencyManager = dependencyManager;
			_dependencyManager.AddSingleton<IUgfApplication>(this);
			_dependencyManager.AddSingleton<IModuleContainer>(this);
			_dependencyManager.AddSingleton<IModuleLoader>(new ModuleLoader());
			_dependencyManager.AddTypes(
				typeof(DependencyManager),
				typeof(LoadingService),
				typeof(IArchivingContext),
				typeof(DefaultLocalizerFactory),
				typeof(InputService)
			);

			Modules = LoadModules(_dependencyManager, plugInSources);
		}

		public Type StartupModuleType { get; }

		public IDependencyProvider DependencyProvider => _dependencyManager;

		public IReadOnlyList<IUgfModuleDescriptor> Modules { get; }

		public IDependencyScope CreateDependencyScope()
		{
			return _dependencyManager.CreateScope();
		}

		public void Configure()
		{
			CheckMultipleConfigureServices();

			var context = new ConfigurationContext(_dependencyManager);

			foreach (var m in Modules)
				if (m.Instance is IPreConfigure module)
					module.PreConfigureGame(context);

			foreach (var module in Modules)
				module.Instance.ConfigureGame(context);

			foreach (var m in Modules)
				if (m.Instance is IPostConfigure module)
					module.PostConfigureGame(context);

			_configuredServices = true;
		}

		public IEnumerator GameCreate()
		{
			using var scope = CreateDependencyScope();

			InitializationContext context = new(scope.DependencyProvider);

			CreationContext creationContext = new(scope.DependencyProvider);

			var loading = Og.LoadingService;

			loading.MaxValue = Modules.Count * 3;

			foreach (var m in Modules)
			{
				loading.Value++;
				yield return null;

				if (m.Instance is IOnPreInitialization module)
					module.OnGamePreInitialization(context);
			}

			foreach (var m in Modules)
			{
				loading.Value++;
				yield return null;

				if (m.Instance is IOnGameArchiving module)
					module.OnGameCreation(creationContext);
			}

			foreach (var m in Modules)
			{
				loading.Value++;
				yield return null;

				if (m.Instance is IOnPostInitialization module)
					module.OnGamePostInitialization(context);
			}

			loading.Value = loading.MaxValue;
		}

		public IEnumerator GameLoad()
		{
			using var scope = CreateDependencyScope();

			InitializationContext context = new(scope.DependencyProvider);

			using LoadingContext loadingContext = new(scope.DependencyProvider);

			var loading = Og.LoadingService;

			loading.MaxValue = Modules.Count * 3;

			foreach (var m in Modules)
			{
				loading.Value++;
				yield return null;

				if (m.Instance is IOnPreInitialization module)
					module.OnGamePreInitialization(context);
			}

			foreach (var m in Modules)
			{
				loading.Value++;
				yield return null;

				if (m.Instance is IOnGameArchiving module)
					module.OnGameLoading(loadingContext);
			}

			foreach (var m in Modules)
			{
				loading.Value++;
				yield return null;

				if (m.Instance is IOnPostInitialization module)
					module.OnGamePostInitialization(context);
			}

			loading.Value = loading.MaxValue;
		}

		public IEnumerator GameSave()
		{
			using var scope = CreateDependencyScope();

			using SavingContext context = new(scope.DependencyProvider);

			var slot = context.Get<IArchivingContext>().CurrentSlot;

			var path = Og.ArchivingPath;

			var inPath = Path.Combine(path, slot.Id.ToString());
			if (Directory.Exists(inPath))
			{
				string outPath = inPath + "_backup";
				if (Directory.Exists(outPath))
					Directory.Delete(outPath,true);
				FileUtil.CopyFileOrDirectory(inPath, outPath);
			}
			slot.PrepareSlotSaving(context);

			var loading = Og.LoadingService;

			loading.MaxValue = Modules.Count;

			foreach (var m in Modules)
			{
				loading.Value++;
				yield return null;

				if (m.Instance is IOnGameArchiving module)
					module.OnGameSaving(context);
			}

			loading.Value = loading.MaxValue;
		}

		public IEnumerator Shutdown()
		{
			using var scope = CreateDependencyScope();

			var context = new ShutdownContext(scope.DependencyProvider);

			var loading = Og.LoadingService;

			loading.MaxValue = Modules.Count;

			for (var i = Modules.Count - 1; i >= 0; i--)
			{
				loading.Value++;
				yield return null;

				if (Modules[i].Instance is IOnGameShutdown module)
					module.OnGameShutdown(context);
			}

			loading.Value = loading.MaxValue;
		}


		private IReadOnlyList<IUgfModuleDescriptor> LoadModules(
			IDependencyManager manager,
			PlugInSourceList plugInSources)
		{
			return manager
				.Get<IModuleLoader>()
				.LoadModules(
					manager,
					StartupModuleType,
					plugInSources
				);
		}

		private void CheckMultipleConfigureServices()
		{
			if (_configuredServices)
				throw new UgfInitializationException("Services have already been configured!");
		}
	}
}
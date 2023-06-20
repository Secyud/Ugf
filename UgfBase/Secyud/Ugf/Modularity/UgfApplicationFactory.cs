#region

using System;
using Localization;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Modularity
{
    public abstract class UgfApplicationFactory : MonoBehaviour
    {
        public static UgfApplicationFactory Instance { get; private set; }

        [SerializeField] private Camera MainCamera;
        [SerializeField] private Canvas MainCanvas;

        private IUpdateService _updateService;
        protected abstract PlugInSourceList PlugInSourceList { get; }
        protected abstract Type StartUpModule { get; }

        public IUgfApplication Application { get; private set; }
        public IStringLocalizer<DefaultResource> T { get; private set; }
        public ISpriteLocalizer<DefaultResource> S { get; private set; }
        public InitializeManager InitializeManager { get; private set; }
        public Camera Camera => MainCamera;
        public Canvas Canvas => MainCanvas;

        protected virtual void Awake()
        {
            Application = Create(PlugInSourceList);
            Instance = this;
            IDependencyManager provider = Application.DependencyManager;
            _updateService = provider.TryGet<IUpdateService>();
            T = provider.Get<IStringLocalizer<DefaultResource>>();
            S = provider.Get<ISpriteLocalizer<DefaultResource>>();
            InitializeManager = provider.Get<InitializeManager>();
        }

        protected virtual void Update()
        {
            _updateService?.Update();
        }

        public void GameInitialize()
        {
            StartCoroutine(Application.GameInitialization());
        }

        public void GameShutdown()
        {
            Application.Shutdown();
        }

        private IUgfApplication Create(
            PlugInSourceList plugInSources = null)
        {
            IUgfApplication app = new UgfApplication(
                new DependencyManager(), StartUpModule, plugInSources
            );
            app.Configure();
            return app;
        }
    }
}
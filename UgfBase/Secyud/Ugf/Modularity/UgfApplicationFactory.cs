#region

using System;
using Localization;
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
        public Camera Camera => MainCamera;
        public Canvas Canvas => MainCanvas;

        protected virtual void Awake()
        {
            Application = Create(PlugInSourceList);
            Instance = this;
            _updateService = Application.DependencyManager.TryGet<IUpdateService>();
            T = Application.DependencyManager.Get<IStringLocalizer<DefaultResource>>();
            S = Application.DependencyManager.Get<ISpriteLocalizer<DefaultResource>>();
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
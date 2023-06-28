using System;
using Localization;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using UnityEngine;

namespace Secyud.Ugf.Modularity
{
    public class UgfApplicationFactory
    {
        public  UgfGameManager Manager { get; private set; }
        public IUgfApplication Application { get; private set; }
        public IStringLocalizer<DefaultResource> T { get; private set; }
        public ISpriteLocalizer<DefaultResource> S { get; private set; }
        public InitializeManager InitializeManager { get; private set; }
        public static UgfApplicationFactory Instance { get; private set; }

        public void GameInitialize()
        {
            Manager.StartCoroutine(Application.GameInitialization());
        }

        public void SaveGame()
        {
            Manager.StartCoroutine(Application.GameSaving());
        }

        public void GameShutdown()
        {
            Application.Shutdown();
        }
    
        public IUgfApplication Create(UgfGameManager manager,Type startUpModule,
            PlugInSourceList plugInSources = null)
        {
            Manager = manager;
            IUgfApplication app = new UgfApplication(
                new DependencyManager(), startUpModule, plugInSources
            );
            app.Configure();
            Instance = this;
            Application = app;
            IDependencyManager provider = Application.DependencyManager;
            T = provider.Get<IStringLocalizer<DefaultResource>>();
            //S = provider.Get<ISpriteLocalizer<DefaultResource>>();
            InitializeManager = provider.Get<InitializeManager>();
        
            return app;
        }
    }
}
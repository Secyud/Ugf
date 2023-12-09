using System;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public class UgfApplicationFactory
    {
        public  UgfGameManager Manager { get; private set; }
        public IUgfApplication Application { get; private set; }
        public static UgfApplicationFactory Instance { get; private set; }

        public void InitializeGame()
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
            Instance = this;
            IUgfApplication app = new UgfApplication(
                new DependencyManager(), startUpModule, plugInSources
            );
            Application = app;
            app.Configure();
            return app;
        }
    }
}
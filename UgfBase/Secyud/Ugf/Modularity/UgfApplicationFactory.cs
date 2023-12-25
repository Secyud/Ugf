using System;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public class UgfApplicationFactory
    {
        public IUgfGameManager Manager { get; private set; }
        public IUgfApplication Application { get; private set; }
        public static UgfApplicationFactory Instance { get; private set; }

        public void Shutdown()
        {
            Application.Shutdown();
        }

        public IUgfApplication Create(IUgfGameManager manager, Type startUpModule,
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
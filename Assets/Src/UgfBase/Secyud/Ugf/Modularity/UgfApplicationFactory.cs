#region

using Secyud.Ugf.DependencyInjection;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Modularity
{
    public abstract class UgfApplicationFactory<TStartupModule> : MonoBehaviour
        where TStartupModule : IUgfModule

    {
        private static UgfApplicationFactory<TStartupModule> _factory;

        public IUgfApplication Application { get; private set; }

        protected abstract PlugInSourceList PlugInSourceList { get; }

        protected virtual void Awake()
        {
            Application = Create(PlugInSourceList);
            _factory = this;
        }

        public static void Initialize()
        {
            _factory.StartCoroutine(_factory.Application.GameCreate());
        }

        public static void GameCreate()
        {
            _factory.StartCoroutine(_factory.Application.GameCreate());
        }

        public static void GameLoad()
        {
            _factory.StartCoroutine(_factory.Application.GameLoad());
        }

        public static void GameSaving()
        {
            _factory.StartCoroutine(_factory.Application.GameSave());
        }

        private IUgfApplication Create(
            PlugInSourceList plugInSources = null)
        {
            IUgfApplication app = new UgfApplication(
                new DependencyManager(), typeof(TStartupModule), plugInSources);
            app.Configure();
            return app;
        }
    }
}
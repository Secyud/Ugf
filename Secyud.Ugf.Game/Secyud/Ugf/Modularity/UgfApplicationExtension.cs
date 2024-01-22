using System.Collections;

namespace Secyud.Ugf.Modularity
{
    public static class UgfApplicationExtension
    {
        public static IEnumerator NewingGame(this IUgfApplication application)
        {
            foreach (IUgfModuleDescriptor module in application.Modules)
            {
                if (module.Instance is IGameModule gameModule)
                {
                    yield return gameModule.OnGameNewing();
                }
            }
        }

        public static IEnumerator LoadingGame(this IUgfApplication application)
        {
            foreach (IUgfModuleDescriptor module in application.Modules)
            {
                if (module.Instance is IGameModule gameModule)
                {
                    yield return gameModule.OnGameLoading();
                }
            }
        }

        public static IEnumerator SavingGame(this IUgfApplication applications)
        {
            foreach (IUgfModuleDescriptor descriptor in applications.Modules)
            {
                if (descriptor.Instance is IGameModule gameModule)
                {
                    yield return gameModule.OnGameSaving();
                }
            }
        }

        public static IEnumerator PreInitializeGame(this IUgfApplication application)
        {
            foreach (IUgfModuleDescriptor module in application.Modules)
            {
                if (module.Instance is IGamePreInitialize gameModule)
                {
                    yield return gameModule.OnGamePreInitialize();
                }
            }
        }

        public static IEnumerator PostInitializeGame(this IUgfApplication applications)
        {
            foreach (IUgfModuleDescriptor descriptor in applications.Modules)
            {
                if (descriptor.Instance is IGamePostInitialize gameModule)
                {
                    yield return gameModule.OnGamePostInitialize();
                }
            }
        }
    }
}
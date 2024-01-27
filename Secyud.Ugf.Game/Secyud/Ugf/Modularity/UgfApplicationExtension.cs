using System.Collections;
using Secyud.Ugf.Unity.LoadingManagement;

namespace Secyud.Ugf.Modularity
{
    public static class UgfApplicationExtension
    {
        public static IEnumerator NewingGame(
            this IUgfApplication application,
            IProgressRate progressRate)
        {
            yield return application.PreInitializeGame();
            int moduleCount = application.Modules.Count;
            progressRate.Rate = 0;
            float addStep = 99f / moduleCount;
            for (int i = 0; i < moduleCount; i++)
            {
                if (application.Modules[i].Instance is
                    IGameModule gameModule)
                {
                    yield return gameModule.OnGameNewing();
                }

                progressRate.Rate += addStep;
            }

            yield return application.PostInitializeGame();
            progressRate.Rate = 100;
        }

        public static IEnumerator LoadingGame(
            this IUgfApplication application,
            IProgressRate progressRate)
        {
            yield return application.PreInitializeGame();
            int moduleCount = application.Modules.Count;
            progressRate.Rate = 0;
            float addStep = 99f / moduleCount;
            for (int i = 0; i < moduleCount; i++)
            {
                if (application.Modules[i].Instance is
                    IGameModule gameModule)
                {
                    yield return gameModule.OnGameLoading();
                }

                progressRate.Rate += addStep;
            }

            yield return application.PostInitializeGame();

            progressRate.Rate = 100;
        }

        public static IEnumerator SavingGame(
            this IUgfApplication application,
            IProgressRate progressRate)
        {
            int moduleCount = application.Modules.Count;
            float addStep = 99f / moduleCount;
            for (int i = 0; i < moduleCount; i++)
            {
                if (application.Modules[i].Instance is
                    IGameModule gameModule)
                {
                    yield return gameModule.OnGameSaving();
                }

                progressRate.Rate += addStep;
            }

            progressRate.Rate = 100;
        }

        private static IEnumerator PreInitializeGame(
            this IModuleContainer application)
        {
            int moduleCount = application.Modules.Count;
            for (int i = 0; i < moduleCount; i++)
            {
                if (application.Modules[i].Instance is
                    IGamePreInitialize gameModule)
                {
                    yield return gameModule.OnGamePreInitialize();
                }
            }
        }

        private static IEnumerator PostInitializeGame(
            this IModuleContainer application)
        {
            int moduleCount = application.Modules.Count;
            for (int i = 0; i < moduleCount; i++)
            {
                if (application.Modules[i].Instance is
                    IGamePostInitialize gameModule)
                {
                    yield return gameModule.OnGamePostInitialize();
                }
            }
        }
    }
}
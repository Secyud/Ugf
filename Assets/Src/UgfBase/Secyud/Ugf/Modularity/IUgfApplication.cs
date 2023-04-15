#region

using System;
using System.Collections;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public interface IUgfApplication : IModuleContainer
    {
        Type StartupModuleType { get; }

        IDependencyProvider DependencyProvider { get; }

        IDependencyScope CreateDependencyScope();

        void Configure();

        IEnumerator GameCreate();
        IEnumerator GameLoad();
        IEnumerator GameSave();
        IEnumerator Shutdown();
    }
}
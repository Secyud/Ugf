#region

using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections;

#endregion

namespace Secyud.Ugf.Modularity
{
    public interface IUgfApplication : IModuleContainer
    {
        IDependencyManager DependencyManager { get; }

        int TotalStep { get; }

        int CurrentStep { get; set; }

        void Configure();

        IEnumerator GameInitialization();
        
        IEnumerator GameSaving();

        void Shutdown();
    }
}
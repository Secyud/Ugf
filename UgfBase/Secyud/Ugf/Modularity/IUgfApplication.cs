#region

using Secyud.Ugf.DependencyInjection;
using System.Collections;

#endregion

namespace Secyud.Ugf.Modularity
{
    public interface IUgfApplication : IModuleContainer,IRegistry
    {
        IDependencyManager DependencyManager { get; }

        int TotalStep { get; }

        int CurrentStep { get; set; }

        void Configure();
        void Shutdown();
    }
}
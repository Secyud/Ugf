using Secyud.Ugf.DataManager;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;

namespace Secyud.Ugf.Unity.Utilities
{
    public interface IUnityUtility
    {
        public TypeManager TypeManager { get; }
        public IUgfStringLocalizer StringLocalizer { get; }
        public UgfGameManager GameManager { get; }
        public IDependencyProvider DependencyProvider { get; }
        public IScopeManager ScopeManager { get; }
        public string ApplicationPath { get; }
    }
}
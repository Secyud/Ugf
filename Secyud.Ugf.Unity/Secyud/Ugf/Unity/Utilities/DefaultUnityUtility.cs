using Secyud.Ugf.DataManager;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using UnityEngine;

namespace Secyud.Ugf.Unity.Utilities
{
    public class DefaultUnityUtility:IUnityUtility
    {
        public DefaultUnityUtility(
             UgfGameManager gameManager, 
             IDependencyProvider dependencyProvider)
        {
            GameManager = gameManager;
            DependencyProvider = dependencyProvider;
            StringLocalizer = dependencyProvider.Get<IStringLocalizer>();
            TypeManager = dependencyProvider.Get<TypeManager>();
            ScopeManager = dependencyProvider.Get<IScopeManager>();
            ApplicationPath = Application.dataPath + "/..";
        }

        public TypeManager TypeManager { get; }
        public IStringLocalizer StringLocalizer { get; }
        public UgfGameManager GameManager { get; }
        public IDependencyProvider DependencyProvider { get; }
        public IScopeManager ScopeManager { get; }
        public string ApplicationPath { get; }
    }
}
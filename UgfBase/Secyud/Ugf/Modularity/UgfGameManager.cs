#region

using System;
using Localization;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity.Plugins;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Modularity
{
    public abstract class UgfGameManager : MonoBehaviour
    {
        [SerializeField] private Camera MainCamera;
        [SerializeField] private Canvas MainCanvas;

        protected abstract Type StartUpModule { get; }
        protected abstract PlugInSourceList PlugInSourceList { get; }
        private IUpdateService _updateService;
        private UgfApplicationFactory _factory;
        public Camera Camera => MainCamera;
        public Canvas Canvas => MainCanvas;
        

        public virtual void Awake()
        {
            _factory = new UgfApplicationFactory();
            _factory.Create(this, StartUpModule, PlugInSourceList);
            _updateService = _factory.Application.DependencyManager.Get<IUpdateService>();
        }

        protected virtual void Update()
        {
            _updateService?.Update();
        }
    }
}
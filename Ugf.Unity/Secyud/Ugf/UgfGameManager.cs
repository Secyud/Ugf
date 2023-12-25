#region

using System;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.UpdateComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf
{
    public abstract class UgfGameManager : MonoBehaviour, IUgfGameManager
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
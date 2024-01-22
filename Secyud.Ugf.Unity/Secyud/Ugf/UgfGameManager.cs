using System;
using Secyud.Ugf.Modularity;
using UnityEngine;

namespace Secyud.Ugf
{
    public abstract class UgfGameManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Canvas _canvas;

        protected abstract Type StartUpModule { get; }
        protected abstract PlugInSourceList PlugInSourceList { get; }
        private UgfApplicationFactory _factory;

        public Camera Camera => _camera;
        public Canvas Canvas => _canvas;

        public static UgfGameManager Instance { get; private set; }

        public virtual void Awake()
        {
            if (Instance)
            {
                throw new Exception("Cannot exist two game manager;");
            }

            Instance = this;
            _factory = new UgfApplicationFactory(StartUpModule, PlugInSourceList);
            _factory.Configure();
        }
    }
}
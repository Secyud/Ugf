using System;
using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.UserInterface
{
    public class UiManager : IUiManager, ISingleton
    {

        private readonly Dictionary<string, UiDescriptor> _uis = new();

        private GameObject _canvas;
        public GameObject Canvas => _canvas ??= GameObject.Find("Canvas");

        public void RegisterUis(IEnumerable<string> allUi)
        {
            foreach (var ui in allUi)
                RegisterUi(ui);
        }

        public void RegisterUi(string path)
        {
            var descriptor = new UiDescriptor(path, CreateGameObject);
            _uis[descriptor.Name] = descriptor;
        }

        internal UiDescriptor GetDescriptor(string name)
        {
            return _uis[name];
        }

        private GameObject CreateGameObject(UiDescriptor descriptor)
        {
            if (Canvas is null)
            {
                Debug.LogError("Cannot find 'Canvas'. \r\nPlease check hierarchy.");
                return null;
            }
            
            var ui = Object.Instantiate(
                Resources.Load<GameObject>(descriptor.Path),
                _canvas.transform);

            ui.name = descriptor.Name;

            return ui;
        }
    }
}
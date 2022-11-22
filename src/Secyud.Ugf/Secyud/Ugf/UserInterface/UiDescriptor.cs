using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.UserInterface
{
    internal class UiDescriptor
    {
        public string Name { get; }
        
        public string Path { get; }
        
        public GameObject Instance { get; private set; }

        private readonly Func<UiDescriptor,GameObject> _instanceFactory; 

        public UiDescriptor(string path,Func<UiDescriptor,GameObject> instanceFactory)
        {
            Path = path;
            Name = path[(path.LastIndexOf('/')+1)..];
            Instance = null;
            _instanceFactory = instanceFactory;
        }

        public void CreateSingleton()
        {
            Instance ??= _instanceFactory(this);
        }

        public void Destroy()
        {
            if (Instance is null)
                return;
            Object.Destroy(Instance);
            Instance = null;
        }
    }
}
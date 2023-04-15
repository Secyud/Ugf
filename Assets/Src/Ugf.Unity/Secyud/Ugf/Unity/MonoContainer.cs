#region

using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity
{
    public class MonoContainer<TComponent> where TComponent : MonoBehaviour
    {
        private readonly Func<TComponent> _prefabGetter;
        private TComponent _prefab;
        private TComponent _instance;
        private readonly bool _onCanvas;
        public TComponent Get => _instance;

        public TComponent Create()
        {
            if (!_prefab)
                _prefab = _prefabGetter();
            if (_instance)
                _instance.Destroy();
            _instance = _onCanvas ? _prefab.InstantiateOnCanvas() : _prefab.Instantiate();

            return _instance;
        }

        public void Destroy()
        {
            if (_instance)
                _instance.Destroy();
            _instance = null;
        }

        public MonoContainer(Func<TComponent> prefabGetter, bool onCanvas = true)
        {
            _prefabGetter = prefabGetter;
            _onCanvas = onCanvas;
            _prefab = null;
            _instance = null;
        }
    }
}
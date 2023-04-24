#region

using System;
using Secyud.Ugf.AssetBundles;
using UnityEngine;

#endregion

namespace Secyud.Ugf
{
    public class MonoContainer<TComponent> : ObjectContainer<TComponent> where TComponent : Component
    {
        private readonly bool _onCanvas;
        private TComponent _prefab;

        public MonoContainer(Func<TComponent> getter, bool onCanvas = true)
            : base(getter)
        {
            _onCanvas = onCanvas;
            _prefab = null;
        }

        public MonoContainer(AssetBundleBase abBase, string name, bool onCanvas = true)
            : base(() => abBase.Load<TComponent>(name))
        {
            _onCanvas = onCanvas;
            _prefab = null;
        }

        public override TComponent Value => Instance;

        public TComponent Create()
        {
            if (!_prefab)
            {
                _prefab = Getter();
                if (!_prefab)
                    Debug.LogError($"{typeof(TComponent)} cannot be found!");
            }

            if (Instance)
                Instance.Destroy();
            Instance = _onCanvas ? _prefab.InstantiateOnCanvas() : _prefab.Instantiate();

            return Instance;
        }

        public void Destroy()
        {
            if (Instance) Instance.Destroy();
            Instance = null;
        }
    }
}
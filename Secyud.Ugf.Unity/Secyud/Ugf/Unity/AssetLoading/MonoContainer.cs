using System;
using UnityEngine;

namespace Secyud.Ugf.Unity.AssetLoading
{
    public class MonoContainer<TComponent> : IObjectContainer<TComponent>
        where TComponent : Component
    {
        private readonly Transform _parent;
        private IObjectContainer<TComponent> _prefabContainer;

        private TComponent _instance;

        public MonoContainer(Transform parent)
        {
            _parent = parent;
        }

        public static MonoContainer<TComponent> OnCanvas() => new(U.Canvas.transform);

        public void Register(IObjectContainer<TComponent> prefab)
        {
            _prefabContainer = prefab;
        }

        public TComponent GetValue()
        {
            if (!_instance)
                _instance = _prefabContainer.GetValue().Instantiate(_parent);
            return _instance;
        }

        public void GetValueAsync(Action<TComponent> callback)
        {
            if (_instance)
            {
                callback.Invoke(_instance);
            }
            else
            {
                if (_prefabContainer is null)
                {
                    throw new UgfNotRegisteredException(
                        nameof(MonoContainer<TComponent>),
                        nameof(_prefabContainer));
                }

                _prefabContainer.GetValueAsync(p =>
                {
                    if (!_instance)
                        _instance = p.Instantiate(_parent);
                    callback?.Invoke(_instance);
                });
            }
        }

        public void Destroy()
        {
            _instance.Destroy();
        }
    }
}
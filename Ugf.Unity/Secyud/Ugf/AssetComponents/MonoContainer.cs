#region

using System;
using Secyud.Ugf.AssetLoading;
using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetComponents
{
    public class MonoContainer<TComponent> : AssetContainer<TComponent>, IMonoContainer<TComponent>
        where TComponent : MonoBehaviour
    {
        protected bool OnCanvas;
        protected TComponent Prefab;

        protected MonoContainer()
        {
        }

        public static MonoContainer<TComponent> Create(
            IAssetLoader loader,
            string prefabName = null, bool onCanvas = true)
        {
            return new MonoContainer<TComponent>
            {
                Loader = loader,
                AssetName = prefabName ?? U.TypeToPath<TComponent>() + ".prefab",
                OnCanvas = onCanvas
            };
        }

        public static MonoContainer<TComponent> Create(
            Type loaderType,
            string prefabName = null, bool onCanvas = true)
        {
            return Create(U.Get(loaderType) as IAssetLoader, prefabName, onCanvas);
        }

        public static MonoContainer<TComponent> Create<TAssetLoader>(
            string prefabName = null, bool onCanvas = true)
            where TAssetLoader : class, IAssetLoader
        {
            return Create(U.Get<TAssetLoader>(), prefabName, onCanvas);
        }


        protected override TComponent GetObject()
        {
            GameObject obj = Loader.LoadAsset<GameObject>(AssetName);
            return obj ? obj.GetComponent<TComponent>() : null;
        }

        public override TComponent Value => CurrentInstance;

        public TComponent Create()
        {
            if (!Prefab)
            {
                Prefab = GetObject();
                if (!Prefab)
                    Debug.LogError($"{typeof(TComponent)} cannot be found!");
            }

            if (CurrentInstance)
            {
                CurrentInstance.Destroy();
            }

            CurrentInstance = OnCanvas ? Prefab.InstantiateOnCanvas() : Prefab.Instantiate();

            return CurrentInstance;
        }

        public void Destroy()
        {
            if (CurrentInstance) CurrentInstance.Destroy();
            CurrentInstance = null;
        }
    }
}
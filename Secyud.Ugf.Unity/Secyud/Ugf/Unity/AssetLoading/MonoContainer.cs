#region

using System;
using Secyud.Ugf.Logging;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.AssetLoading
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
            
            if (obj is null)
            {
                UgfLogger.LogError($"Failed get {AssetName}.");
            }

            return obj ? obj.GetComponent<TComponent>() : null;
        }

        public override TComponent Value => CurrentInstance;

        public TComponent Create()
        {
            if (!Prefab)
            {
                Prefab = GetObject();
                if (!Prefab)
                {
                    UgfLogger.LogError($"{typeof(TComponent)} cannot be found!");
                }
            }

            if (CurrentInstance)
            {
                CurrentInstance.Destroy();
            }

            CurrentInstance = OnCanvas ? Prefab.Instantiate(U.Canvas.transform) : Prefab.Instantiate();

            return CurrentInstance;
        }

        public TComponent GetOrCreate()
        {
            if (!Value)
            {
                Create();
            }

            return Value;
        }

        public void Destroy()
        {
            if (CurrentInstance)
            {
                CurrentInstance.Destroy();
            }
        }
    }
}
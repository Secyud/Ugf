#region

using System;
using System.IO;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.Logging;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.AssetLoading
{
    public class PrefabContainer<TComponent> : ObjectContainer<TComponent, GameObject>
        where TComponent : Component
    {
        [S] protected IAssetLoader Loader;
        [S] protected string AssetName;

        protected PrefabContainer()
        {
        }

        public static PrefabContainer<TComponent> Create(
            IAssetLoader loader,
            string prefabName = null)
        {
            return new PrefabContainer<TComponent>
            {
                Loader = loader,
                AssetName = prefabName ?? U.TypeToPath<TComponent>() + ".prefab"
            };
        }

        public static PrefabContainer<TComponent> Create(
            Type loaderType,
            string prefabName = null)
        {
            return Create(U.Get(loaderType) as IAssetLoader, prefabName);
        }

        public static PrefabContainer<TComponent> Create<TAssetLoader>(string prefabName = null)
            where TAssetLoader : class, IAssetLoader
        {
            return Create(U.Get<TAssetLoader>(), prefabName);
        }

        protected override TComponent HandleResult(GameObject result)
        {
            if (!result)
            {
                UgfLogger.LogError($"Failed get object {AssetName}.");
                return null;
            }

            if (!result.TryGetComponent(out TComponent component))
            {
                UgfLogger.LogError($"{typeof(TComponent)} " +
                                   $"doesn't exist on object {AssetName}.");
                return null;
            }

            return component;
        }

        protected override GameObject GetOrigin()
        {
            return Loader.LoadAsset<GameObject>(AssetName);
        }

        protected override void GetOriginAsync(Action<GameObject> useAction)
        {
            Loader.LoadAssetAsync(AssetName, useAction);
        }

        public override TComponent GetValue()
        {
            if (!Instance)
            {
                Instance = HandleResult(GetOrigin());
            }
            return Instance;
        }

        public override void GetValueAsync(Action<TComponent> useAction)
        {
            if (Instance)
            {
                useAction.Invoke(Instance);
            }
            else
            {
                GetOriginAsync(o =>
                {
                    Instance = HandleResult(o);
                    useAction.Invoke(Instance);
                });
            }
        }

        public override void Save(BinaryWriter writer)
        {
            writer.WriteNullable(Loader);
            writer.Write(AssetName);
        }

        public override void Load(BinaryReader reader)
        {
            Loader = reader.ReadNullable<IAssetLoader>();
            AssetName = reader.ReadString();
        }
    }
}
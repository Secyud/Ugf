using System;
using System.IO;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.Logging;
using UnityEngine;

namespace Secyud.Ugf.Unity.AssetLoading
{
    public class PrefabContainer<TComponent> : ObjectContainer<TComponent, GameObject>
        where TComponent : Component
    {
        [S] protected IAssetLoader AssetLoader;
        [S] protected string AssetName;
        protected virtual IAssetLoader Loader => AssetLoader;

        protected PrefabContainer()
        {
        }

        public static PrefabContainer<TComponent> Create(
            IAssetLoader loader,
            string prefabName = null)
        {
            return new PrefabContainer<TComponent>
            {
                AssetLoader = loader,
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

        protected override TComponent GetValueFromOrigin(GameObject result)
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

        protected override bool CheckInstance()
        {
            return Instance;
        }

        protected override GameObject GetOrigin()
        {
            return Loader.LoadAsset<GameObject>(AssetName);
        }

        protected override void GetOriginAsync(Action<GameObject> callback)
        {
            Loader.LoadAssetAsync(AssetName, callback);
        }

        public override void Save(BinaryWriter writer)
        {
            writer.WriteNullable(Loader);
            writer.Write(AssetName);
        }

        public override void Load(BinaryReader reader)
        {
            AssetLoader = reader.ReadNullable<IAssetLoader>();
            AssetName = reader.ReadString();
        }
    }
}
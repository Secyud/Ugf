﻿using System;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.AssetBundles
{
    public abstract class AssetBundleBase : ISingleton
    {
        protected AssetBundleBase()
        {
            AssetBundle = new ObjectContainer<AssetBundle>(() =>
                Og.Get<AssetBundleManager>().GetByPath(AssetBundleName));
        }

        public ObjectContainer<AssetBundle> AssetBundle { get; }
        public abstract string AssetBundleName { get; }
        public abstract string AssetBundleRoot { get; }

        public TAsset Load<TAsset>(string name) where TAsset : Component
        {
            GameObject obj = AssetBundle.Value.LoadAsset<GameObject>(name);

            if (obj is null)
                throw new NullReferenceException($"Cannot find asset named {name} in {AssetBundleName}");

            return obj.GetComponent<TAsset>();
        }

        // public TAsset Load<TAsset>(string name) where TAsset : Component 
        //     => AssetBundle.Get.LoadAsset<TAsset>(name);
        public TAsset LoadAsset<TAsset>(string name) where TAsset : Object
        {
            return AssetBundle.Value.LoadAsset<TAsset>(name);
        }
    }
}
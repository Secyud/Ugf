using Secyud.Ugf.Container;
using System;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.AssetBundles
{
    public abstract class AssetBundleBase : AssetBundleContainer, ISingleton
    {
        protected AssetBundleBase(string assetBundleName)
            :base(assetBundleName)
        {
        }
    }
}
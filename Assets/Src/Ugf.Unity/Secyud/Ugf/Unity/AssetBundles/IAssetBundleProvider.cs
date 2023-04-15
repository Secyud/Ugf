#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.AssetBundles
{
    public interface IAssetBundleProvider
    {
        AssetBundle GetByPath(string path);

        void ReleaseByPath(string path);
    }
}
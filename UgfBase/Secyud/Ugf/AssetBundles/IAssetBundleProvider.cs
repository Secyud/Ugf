#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetBundles
{
	public interface IAssetBundleProvider
	{
		AssetBundle GetByPath(string path);

		void ReleaseByPath(string path);
	}
}
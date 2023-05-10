#region

using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.AssetBundles
{
	public abstract class AssetBundleBase : AssetBundleContainer, ISingleton
	{
		protected AssetBundleBase(string assetBundleName)
			: base(assetBundleName)
		{
		}
	}
}
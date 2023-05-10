#region

using Secyud.Ugf.ButtonComponents;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public interface ISorterRegistration<in TTarget> : ICanBeStated
	{
		int SortValue(TTarget target);
	}
}
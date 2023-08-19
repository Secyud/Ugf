
namespace Secyud.Ugf.TableComponents.FilterComponents
{
	public abstract class FilterToggleDescriptor<TItem> : FilterBaseDescriptor
	{
		public abstract bool Filter(TItem target);
	}
}
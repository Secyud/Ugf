#region

using Secyud.Ugf.Collections;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.TableComponents.ButtonComponents
{
	public abstract class ButtonRegeditBase<TItem> :
		RegistrableList<ButtonDescriptor<TItem>>,IRegistry
	{
		
	}
}
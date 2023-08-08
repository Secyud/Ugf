using Secyud.Ugf.Collections;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.TableComponents.SorterComponents
{
    public abstract class SorterRegeditBase<TItem> :
        RegistrableList<SorterToggleDescriptor<TItem>>,IRegistry
    {
    }
}
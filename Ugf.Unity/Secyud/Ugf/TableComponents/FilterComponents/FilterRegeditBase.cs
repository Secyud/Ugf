using Secyud.Ugf.Collections;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public abstract class FilterRegeditBase<TItem>:
        RegistrableList<FilterTriggerDescriptor<TItem>>,IRegistry
    {
        
    }
}
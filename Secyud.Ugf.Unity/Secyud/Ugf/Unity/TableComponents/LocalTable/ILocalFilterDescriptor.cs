using Secyud.Ugf.Unity.TableComponents.UiFunctions;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public interface ILocalFilterDescriptor 
    {
        bool Filter(object target);
    }
}
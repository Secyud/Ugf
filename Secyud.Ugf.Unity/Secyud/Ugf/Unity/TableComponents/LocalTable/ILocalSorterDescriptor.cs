using Secyud.Ugf.Unity.TableComponents.UiFunctions;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public interface ILocalSorterDescriptor 
    {
        object GetSortValue(object obj);
    }
}
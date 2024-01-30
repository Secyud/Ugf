using Secyud.Ugf.Unity.TableComponents.UiFunctions;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public abstract class LocalSorterBase:ILocalSorterDescriptor,ITableSorterDescriptor
    {
        protected LocalSorterBase(string name)
        {
            Name = name;
        }
        
        public abstract object GetSortValue(object obj);
        public bool? State { get; set; }
        public string Name { get; }
    }
}
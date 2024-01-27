using Secyud.Ugf.Unity.TableComponents.UiFunctions;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public abstract class LocalSorterBase:ILocalSorter,ITableSorterDescriptor
    {
        protected LocalSorterBase(string name)
        {
            Name = name;
        }
        
        public abstract int Compare(object left, object right);
        public bool? State { get; set; }
        public string Name { get; }
    }
}
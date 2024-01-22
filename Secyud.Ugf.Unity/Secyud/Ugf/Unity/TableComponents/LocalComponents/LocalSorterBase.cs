using Secyud.Ugf.Unity.TableComponents.Components;

namespace Secyud.Ugf.Unity.TableComponents.LocalComponents
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
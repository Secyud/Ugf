using Secyud.Ugf.Unity.TableComponents.Components;

namespace Secyud.Ugf.Unity.TableComponents.LocalComponents
{
    public abstract class LocalFilterBase:ILocalFilter,ITableFilterDescriptor
    {
        private bool _state;

        protected LocalFilterBase(string name)
        {
            Name = name;
        }
        
        public bool State { get; set; }
        public string Name { get; }
        public abstract bool Filter(object target);
    }
}
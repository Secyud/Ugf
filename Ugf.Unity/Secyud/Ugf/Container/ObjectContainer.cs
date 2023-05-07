using Object = UnityEngine.Object;

namespace Secyud.Ugf.Container
{
    public abstract class ObjectContainer<TObject> : IObjectAccessor<TObject>
    {
        protected TObject Instance;

        protected ObjectContainer()
        {
        }

        protected abstract TObject GetObject();
        
        public virtual TObject Value => Instance ??= GetObject();
    }
}
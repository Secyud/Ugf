using System;
using JetBrains.Annotations;

namespace Secyud.Ugf.Unity.AssetLoading
{
    public class DirectContainer<TObject>:IObjectContainer<TObject>
    {
        private readonly TObject _o;

        public DirectContainer([NotNull]TObject o)
        {
            _o = o;
        }
        
        public TObject GetValue()
        {
            return _o;
        }

        public void GetValueAsync(Action<TObject> callback)
        {
            callback?.Invoke(_o);
        }
    }
}
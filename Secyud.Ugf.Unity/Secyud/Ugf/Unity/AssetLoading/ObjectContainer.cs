using System;
using System.IO;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.Unity.AssetLoading
{
    public abstract class ObjectContainer<TValue, TOrigin> : 
        IObjectContainer<TValue>,IArchivable
    {
        protected TValue Instance { get; set; }

        protected ObjectContainer()
        {
        }

        protected abstract TValue HandleResult(TOrigin result);
        protected abstract TOrigin GetOrigin();
        protected abstract void GetOriginAsync(Action<TOrigin> callback);

        public virtual TValue GetValue()
        {
            Instance ??= HandleResult(GetOrigin());
            return Instance;
        }

        public virtual void GetValueAsync(Action<TValue> callback)
        {
            if (Instance is not null)
            {
                callback.Invoke(Instance);
            }
            else
            {
                GetOriginAsync(o =>
                {
                    Instance = HandleResult(o);
                    callback.Invoke(Instance);
                });
            }
        }

        public abstract void Save(BinaryWriter writer);

        public abstract void Load(BinaryReader reader);
    }
}
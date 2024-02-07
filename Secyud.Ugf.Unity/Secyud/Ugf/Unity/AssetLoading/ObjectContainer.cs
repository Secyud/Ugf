using System;
using System.IO;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.Unity.AssetLoading
{
    public abstract class ObjectContainer<TValue, TOrigin> :
        IObjectContainer<TValue>, IArchivable
    {
        protected TValue Instance { get; set; }

        protected bool AsyncOperated { get; set; }

        protected ObjectContainer()
        {
        }

        protected abstract TValue GetValueFromOrigin(TOrigin result);
        protected abstract TOrigin GetOrigin();
        protected abstract void GetOriginAsync(Action<TOrigin> callback);

        private event Action<TValue> ValueOperation;

        protected virtual void SetInstanceWithOrigin(TOrigin origin)
        {
            Instance = GetValueFromOrigin(origin);
            ValueOperation?.Invoke(Instance);
            ValueOperation = null;
        }

        protected virtual bool CheckInstance()
        {
            return Instance is not null;
        }

        public virtual TValue GetValue()
        {
            if (!CheckInstance())
            {
                SetInstanceWithOrigin(GetOrigin());
            }

            return Instance;
        }

        public virtual void GetValueAsync(Action<TValue> callback)
        {
            if (CheckInstance())
            {
                ValueOperation += callback;
                if (!AsyncOperated)
                {
                    GetOriginAsync(SetInstanceWithOrigin);
                    AsyncOperated = true;
                }
            }
            else
            {
                callback?.Invoke(Instance);
            }
        }

        public abstract void Save(BinaryWriter writer);

        public abstract void Load(BinaryReader reader);
    }
}
using System;

namespace Secyud.Ugf.Unity.AssetLoading
{
    public interface IObjectContainer<out T>
    {
        T GetValue();
        void GetValueAsync(Action<T> callback);
    }
}
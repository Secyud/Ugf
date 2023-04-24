using System;
using System.IO;
using Secyud.Ugf.AssetBundles;
using Object = UnityEngine.Object;

namespace Secyud.Ugf
{
    public class ObjectContainer<TObject> : IObjectAccessor<TObject> where TObject : Object
    {
        protected readonly Func<TObject> Getter;
        protected TObject Instance;


        public ObjectContainer(Func<TObject> getter)
        {
            Getter = getter;
            Instance = null;
        }

        public ObjectContainer(AssetBundleBase abBase, string iconPath, bool path = false)
        {
            if (path) iconPath = Path.Combine(abBase.AssetBundleRoot, iconPath);
            Getter = () => abBase.LoadAsset<TObject>(iconPath);
            Instance = null;
        }

        public virtual TObject Value => Instance ? Instance : Instance = Getter();
    }
}
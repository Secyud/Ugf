using System;
using System.Collections.Generic;
using System.Reflection;

namespace Secyud.Ugf.Unity.Prefabs
{
    public interface IPrefabRegistrar
    {
        public void RegisterPrefabsInAssembly(Assembly prefabAssembly, bool isUi = true,string assetBundlePath = null);

        void RegisterPrefabs(IEnumerable<Type> prefabTypes, bool isUi = true,string assetBundlePath = null);

        void RegisterPrefab(Type prefabType, bool isUi = true,string assetBundlePath = null);
    }
}
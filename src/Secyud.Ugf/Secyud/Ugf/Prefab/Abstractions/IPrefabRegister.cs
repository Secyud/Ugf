using System;
using System.Collections.Generic;
using System.Reflection;

namespace Secyud.Ugf.Prefab
{
    public interface IPrefabRegister
    {
        public void RegisterPrefabsInAssembly(Assembly prefabAssembly, bool isUi = false);
        
        void RegisterPrefabs(IEnumerable<Type> prefabTypes, bool isUi = false);
        
        void RegisterPrefab(Type prefabType, bool isUi = false);
    }
}
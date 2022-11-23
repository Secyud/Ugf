using System.Collections.Generic;

namespace Secyud.Ugf.Prefab
{
    public interface IPrefabManager
    {
        public void RegisterPrefabsInFolder(string path, bool isUi = false);
        void RegisterPrefabs(IEnumerable<string> prefabs, bool isUi = false);
        void RegisterPrefab(string path, bool isUi = false);
    }
}
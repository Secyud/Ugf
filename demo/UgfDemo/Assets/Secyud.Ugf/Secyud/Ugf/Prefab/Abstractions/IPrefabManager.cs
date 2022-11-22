using System.Collections.Generic;

namespace Secyud.Ugf.Prefab
{
    public interface IPrefabManager
    {
        void RegisterPrefabs(IEnumerable<string> prefabs, bool isUi = false);
        void RegisterPrefab(string path, bool isUi = false);
    }
}
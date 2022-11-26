using System.Linq;

namespace UnityEngine
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject)
            where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }


        public static GameObject GetSubObject(this GameObject gameObject, string name)
        {
            return gameObject
                .GetComponentsInChildren<Transform>()
                .FirstOrDefault(u => u.name == name)?
                .gameObject;
        }
    }
}
namespace UnityEngine
{
    public static class UgfUnityGameObjectExtensions
    {
        public static TComponent Instantiate<TComponent>(this TComponent template, Transform parent = null)
            where TComponent : Component
        {
            return parent is null ? Object.Instantiate(template) : Object.Instantiate(template, parent);
        }

        public static void Destroy<TComponent>(this TComponent component)
            where TComponent : Component
        {
            Object.Destroy(component);
        }

        public static T GetOrAddComponent<T>(this Component origin)
            where T : Component
        {
            if (!origin.TryGetComponent(out T component))
                component = origin.gameObject.AddComponent<T>();
            return component;
        }
    }
}
namespace System.Collections.Generic
{
    public static class UgfUnityListExtensions
    {
        public static T RandomPick<T>(this IList<T> source)
        {
            return source.IsNullOrEmpty()
                ? default
                : source[UnityEngine.Random.Range(0, source.Count)];
        }
    }
}
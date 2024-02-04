using System.Collections;
using Secyud.Ugf.Abstraction;

namespace UnityEngine
{
    public static class UgfTransformExtension
    {
        public static void TryFillWithContent(this Transform transform, object mayHasContent)
        {
            if (transform &&
                mayHasContent is IHasContent hasContent)
            {
                hasContent.SetContent(transform);
            }
        }

        public static void TryFillWithContents(this Transform transform, IEnumerable mayHasContentList)
        {
            foreach (object item in mayHasContentList)
            {
                transform.TryFillWithContent(item);
            }
        }
    }
}
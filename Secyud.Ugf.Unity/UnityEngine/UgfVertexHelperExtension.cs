using UnityEngine.UI;

namespace UnityEngine
{
    public static class UgfVertexHelperExtension
    {
        public static void AddRectangle(this VertexHelper vh,
            int i0, int i1, int i2, int i3)
        {
            vh.AddTriangle(i0, i1, i3);
            vh.AddTriangle(i1, i2, i3);
        }
    }
}
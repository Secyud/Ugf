#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.HexUtilities
{
    /// <summary>
    ///     Five-component hash value.
    /// </summary>
    public struct HexHash
    {
        public float A { get; }
        public float B { get; }
        public float C { get; }
        public float D { get; }
        public float E { get; }

        private HexHash(float a, float b, float c, float d, float e)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
        }

        /// <summary>
        ///     Create a hex hash.
        /// </summary>
        /// <returns>Hash value based on <see cref="UnityEngine.Random" />.</returns>
        public static HexHash Create()
        {
            return new HexHash(Random.value * 0.999f,
                Random.value * 0.999f,
                Random.value * 0.999f,
                Random.value * 0.999f,
                Random.value * 0.999f);
        }
    }
}
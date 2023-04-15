#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    /// Five-component hash value.
    /// </summary>
    public struct HexHash
    {
        public float A, B, C, D, E;

        /// <summary>
        /// Create a hex hash.
        /// </summary>
        /// <returns>Hash value based on <see cref="UnityEngine.Random"/>.</returns>
        public static HexHash Create()
        {
            HexHash hash;
            hash.A = Random.value * 0.999f;
            hash.B = Random.value * 0.999f;
            hash.C = Random.value * 0.999f;
            hash.D = Random.value * 0.999f;
            hash.E = Random.value * 0.999f;
            return hash;
        }
    }
}
using UnityEngine;

namespace Secyud.Ugf.Modularity
{
    public interface IUgfGameManager
    {
        Camera Camera { get; }
        Canvas Canvas { get; }
    }
}
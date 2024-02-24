using Secyud.Ugf.Unity.AssetLoading;
using UnityEngine;

namespace Secyud.Ugf.Abstraction
{
    public interface IHasIcon
    {
        IObjectContainer<Sprite> Icon { get; }
    }
}
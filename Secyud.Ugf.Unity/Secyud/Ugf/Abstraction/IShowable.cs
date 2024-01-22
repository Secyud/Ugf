using Secyud.Ugf.Unity.AssetLoading;
using UnityEngine;

namespace Secyud.Ugf.Abstraction
{
    public interface IHasDescription
    {
        string Description { get; }
    }

    public interface IHasName
    {
        string Name { get; }
    }

    public interface IHasIcon
    {
        IObjectContainer<Sprite> Icon { get; }
    }
}
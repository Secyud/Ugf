using UnityEngine;

namespace Secyud.Ugf
{
    public interface ICanBeShown
    {
        string Name { get; }
        string Description { get; }
        IObjectAccessor<Sprite> Icon { get; }
    }
}
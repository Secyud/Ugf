#region

using UnityEngine;

#endregion

namespace Secyud.Ugf
{
    public interface IHasDescription
    {
        string Description { get; }
    }

    public interface IHasName
    {
        string Name { get; }
    }

    public interface IShowable : IHasDescription, IHasName
    {
        IObjectAccessor<Sprite> Icon { get; }
    }
}
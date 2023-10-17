#region

using UnityEngine;

#endregion

namespace Secyud.Ugf
{
    public interface IHasDescription
    {
        string ShowDescription { get; }
    }

    public interface IHasName
    {
        string ShowName { get; }
    }

    public interface IShowable : IHasDescription, IHasName
    {
        IObjectAccessor<Sprite> ShowIcon { get; }
    }
}
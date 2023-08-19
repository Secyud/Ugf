#region

using Localization;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Localization
{
    public interface ISpriteLocalizer
    {
        Sprite this[string str] { get; }
        Sprite this[string str, params object[] args] { get; }
    }

    // ReSharper disable once UnusedTypeParameter
    public interface ISpriteLocalizer<TResource> : ISpriteLocalizer
        where TResource : DefaultResource
    {
    }
}
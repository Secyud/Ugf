#region

using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity
{
    public class SpriteGetter : ISpriteGetter
    {
        public Sprite Get => _sprite ??= _spriteGetter(this);
        public readonly string Name;
        public readonly string AssetBundlePath;

        private Sprite _sprite;
        private readonly Func<SpriteGetter, Sprite> _spriteGetter;

        public SpriteGetter(string name, string assetBundlePath, Func<SpriteGetter, Sprite> spriteGetter)
        {
            Name = name;
            AssetBundlePath = assetBundlePath;
            _spriteGetter = spriteGetter;
        }
    }
}
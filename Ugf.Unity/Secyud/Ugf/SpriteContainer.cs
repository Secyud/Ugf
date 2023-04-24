using Secyud.Ugf.AssetBundles;
using UnityEngine;

namespace Secyud.Ugf
{
    public class SpriteContainer:IObjectAccessor<Sprite>
    {

        public SpriteContainer(string abName, string spriteName)
        {
            AbName = abName;
            SpriteName = spriteName;
        }

        public readonly string AbName;
        public readonly string SpriteName;
        private Sprite _sprite;

        public Sprite Value
        {
            get
            {
                if (!_sprite)
                {
                    var abPath = Og.GetAbFullPath(AbName);
                    var ab = Og.Get<AssetBundleManager>().GetByPath(abPath);
                    _sprite = ab.LoadAsset<Sprite>($"Assets/AssetBundles/{AbName}/Images/{SpriteName}");
                }

                return _sprite;
            }
        }
    }
}
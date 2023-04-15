#region

using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.AssetBundles
{
    public class SpriteManager : ISingleton
    {
        private readonly IAssetBundleProvider _assetBundleProvider;
        private readonly Dictionary<string, Dictionary<string, ISpriteGetter>> _iconDictionary = new();

        public SpriteManager(IAssetBundleProvider assetBundleProvider)
        {
            _assetBundleProvider = assetBundleProvider;
        }

        public ISpriteGetter GetIcon(string name, string bundlePath)
        {
            if (!_iconDictionary.ContainsKey(bundlePath))
                _iconDictionary[bundlePath] = new();

            Dictionary<string, ISpriteGetter> dict = _iconDictionary[bundlePath];

            if (!dict.ContainsKey(name))
                dict[name] = new SpriteGetter(name, bundlePath, GetSprite);

            return dict[name];
        }

        private Sprite GetSprite(SpriteGetter spriteGetter)
        {
            return _assetBundleProvider.GetByPath(spriteGetter.AssetBundlePath).LoadAsset<Sprite>(spriteGetter.Name);
        }
    }
}
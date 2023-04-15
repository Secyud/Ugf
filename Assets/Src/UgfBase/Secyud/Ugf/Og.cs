#region

using Localization;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using UnityEngine;

#endregion

namespace Secyud.Ugf
{
    public static class Og
    {
        public static readonly Camera MainCamera = Camera.main;
        public static readonly Canvas Canvas = GameObject.Find("StaticObject/Canvas").GetComponent<Canvas>();
        public static readonly Sprite EmptyImage = Resources.Load<Sprite>("Images/empty");

        internal static void Initialize(IDependencyProvider provider)
        {
            Provider = provider;
            L = Get<IStringLocalizer<DefaultResource>>();
            LoadingService = Get<LoadingService>();
        }


        public static IDependencyProvider Provider { get; private set; }
        public static IStringLocalizer<DefaultResource> L { get; private set; }
        public static LoadingService LoadingService { get; private set; }
        public static T Get<T>() where T : class => Provider.Get<T>();
    }
}
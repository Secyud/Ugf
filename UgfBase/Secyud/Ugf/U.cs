#region

using System;
using Localization;
using Secyud.Ugf.AssetLoading;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;
using System.Reflection;
using Secyud.Ugf.Resource;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace Secyud.Ugf
{
    public static class U
    {
        public static IStringLocalizer<DefaultResource> T => UgfApplicationFactory.Instance.T;
        public static ISpriteLocalizer<DefaultResource> S => UgfApplicationFactory.Instance.S;
        public static Camera Camera => UgfApplicationFactory.Instance.Camera;
        public static Canvas Canvas => UgfApplicationFactory.Instance.Canvas;


        public static T Get<T>() where T : class
        {
            return UgfApplicationFactory.Instance.Application.DependencyManager.Get<T>();
        }

        public static int GetRandom(int max, int min = 0)
        {
            return Random.Range(min, max);
        }

        public static string TypeToPath<TObj>()
        {
            return TypeToPath(typeof(TObj));
        }

        public static string TypeToPath(Type type)
        {
            return DotToPath(type.FullName);
        }

        public static string DotToPath(string name)
        {
            return name.Replace('.', '/');
        }
    }
}
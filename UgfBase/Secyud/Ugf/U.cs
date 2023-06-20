#region

using System;
using Localization;
using Secyud.Ugf.AssetLoading;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;
using System.Reflection;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DataManager;
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
        public static UgfApplicationFactory Factory => UgfApplicationFactory.Instance;


        public static T Get<T>() where T : class
        {
            return UgfApplicationFactory.Instance.Application.DependencyManager.Get<T>();
        }

        public static object Get(Type loaderType)
        {
            return UgfApplicationFactory.Instance.Application.DependencyManager.Get(loaderType);
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

        public static void AutoSaveObject(object o, IArchiveWriter writer)
        {
            PropertyDescriptor property = UgfApplicationFactory.Instance
                .InitializeManager.GetProperty(o.GetType());
            
            property.Write(o,writer);
        }
        
        public static void AutoLoadObject(object o, IArchiveReader reader)
        {
            PropertyDescriptor property = UgfApplicationFactory.Instance
                .InitializeManager.GetProperty(o.GetType());
            
            property.Read(o,reader);
        }
    }
}
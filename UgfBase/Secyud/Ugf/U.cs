#region

using System;
using Localization;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;
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
        public static InitializeManager I => UgfApplicationFactory.Instance.InitializeManager;
        public static Camera Camera => UgfApplicationFactory.Instance.Manager.Camera;
        public static Canvas Canvas => UgfApplicationFactory.Instance.Manager.Canvas;
        public static UgfApplicationFactory Factory => UgfApplicationFactory.Instance;
        public static string Path
        {
            get
            {
#if UNITY_//EDITOR
                return Application.dataPath[..^6];
#else
                return System.IO.Directory.GetCurrentDirectory();
#endif
            }
        }

        public static T Get<T>() where T : class
        {
            return UgfApplicationFactory.Instance.Application.DependencyManager.Get<T>();
        }

        public static object Get(Type type)
        {
            return UgfApplicationFactory.Instance.Application.DependencyManager.Get(type);
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
            PropertyDescriptor property = I.GetProperty(o.GetType());
            
            property.Write(o,writer);
        }
        
        public static void AutoLoadObject(object o, IArchiveReader reader)
        {
            PropertyDescriptor property = I.GetProperty(o.GetType());
            
            property.Read(o,reader);
        }

        private static int _step;
        public static bool AddStep(int value = 1)
        {
            if (_step > 1024)
            {
                Factory.Application.CurrentStep+=value;
                _step = 0;
                return true;
            }

            _step++;
            return false;
        }
    }
}
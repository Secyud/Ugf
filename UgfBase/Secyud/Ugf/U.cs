#region

using System;
using Localization;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace Secyud.Ugf
{
    public static class U
    {
        public static IStringLocalizer<DefaultResource> T => UgfApplicationFactory.Instance.T;
        public static ISpriteLocalizer<DefaultResource> S => UgfApplicationFactory.Instance.S;
        public static TypeManager Tm => TypeManager.Instance;
        public static IDependencyManager M => UgfApplicationFactory.Instance.Application.DependencyManager;
        public static Camera Camera => UgfApplicationFactory.Instance.Manager.Camera;
        public static Canvas Canvas => UgfApplicationFactory.Instance.Manager.Canvas;
        public static UgfApplicationFactory Factory => UgfApplicationFactory.Instance;

        public static string Path

        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath[..^6];
#else
                return System.IO.Directory.GetCurrentDirectory();
#endif
            }
        }

        public static T Get<T>() where T : class
        {
            return M.Get<T>();
        }

        public static object Get(Type type)
        {
            return M.Get(type);
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

        private static int _step;

        public static bool AddStep(int value = 1)
        {
            if (_step > 1024)
            {
                Factory.Application.CurrentStep += value;
                _step = 0;
                return true;
            }

            _step++;
            return false;
        }

        public static void AutoSaveObject(object obj, IArchiveWriter writer)
        {
            TypeDescriptor descriptor = Tm.GetProperty(obj.GetType());
            foreach (SAttribute attribute in descriptor.Properties.Attributes[3])
                writer.WriteField(attribute.GetValue(obj), attribute.Type);
        }

        public static void AutoLoadObject(object obj, IArchiveReader reader)
        {
            TypeDescriptor descriptor = Tm.GetProperty(obj.GetType());
            foreach (SAttribute attribute in descriptor.Properties.Attributes[3])
                attribute.SetValue(obj, reader.ReadField(attribute.Type));
        }
    }
}
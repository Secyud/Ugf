#region

using System;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#endregion

namespace Secyud.Ugf
{
    public static class U
    { 
        public static bool DataManager { get; set; } = false;
        public static ILocalizer<string> T => DefaultLocalizer<string>.Localizer;
        public static TypeManager Tm => TypeManager.Instance;
        public static IDependencyManager M => UgfApplicationFactory.Instance.Application.DependencyManager;
        public static Camera Camera => UgfApplicationFactory.Instance.Manager.Camera;
        public static Canvas Canvas => UgfApplicationFactory.Instance.Manager.Canvas;
        public static UgfApplicationFactory Factory => UgfApplicationFactory.Instance;


        public static string Path
        {
            get
            {
                if (DataManager)
                {
                    return System.IO.Directory.GetCurrentDirectory();
                }
                else
                {
                    return Application.dataPath + "/..";
                }
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

        public static void Log(object obj)
        {
#if DEBUG
            if (DataManager)
            {
                Console.WriteLine(obj);
            }
            else
            {
                Debug.Log(obj);
            }
#endif
        }

        public static void LogError(object obj)
        {
            if (DataManager)
            {
                Console.Error.WriteLine(obj);
            }
            else
            {
                Debug.LogError(obj);
            }
        }

        public static void LogError(object obj, Object context)
        {
            if (DataManager)
            {
                Console.Error.WriteLine($"{obj} \r\n {context}");
            }
            else
            {
                Debug.LogError(obj, context);
            }
        }

        public static void LogWarning(object obj)
        {
            if (DataManager)
            {
                Console.WriteLine(obj);
            }
            else
            {
                Debug.LogWarning(obj);
            }
        }
    }
}
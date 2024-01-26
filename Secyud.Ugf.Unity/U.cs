using System;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Unity.Utilities;
using UnityEngine;

public static class U
{
    private static IUnityUtility _unityUtility;

    internal static void SetUtility(IUnityUtility utility)
    {
        _unityUtility = utility;
    }

    public static string Path => _unityUtility.ApplicationPath;
    public static IStringLocalizer T => _unityUtility.StringLocalizer;
    public static TypeManager Tm => _unityUtility.TypeManager;
    public static Camera Camera => _unityUtility.GameManager.Camera;
    public static Canvas Canvas => _unityUtility.GameManager.Canvas;


    public static T Get<T>() where T : class
    {
        return _unityUtility.DependencyProvider.Get<T>();
    }

    public static object Get(Type type)
    {
        return _unityUtility.DependencyProvider.Get(type);
    }

    public static string TypeToPath<T>()
    {
        return TypeToPath(typeof(T));
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
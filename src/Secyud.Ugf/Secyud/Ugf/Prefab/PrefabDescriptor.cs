using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Prefab;

internal class PrefabDescriptor
{
    private readonly Func<PrefabDescriptor, GameObject, GameObject> _instanceFactory;

    public readonly bool IsUi;

    public PrefabDescriptor(string path, Func<PrefabDescriptor, GameObject, GameObject> instanceFactory, bool isUi)
    {
        Path = path;
        Name = path[(path.LastIndexOf('/') + 1)..];
        Instance = null;
        _instanceFactory = instanceFactory;
        IsUi = isUi;
    }

    public string Name { get; }

    public string Path { get; }

    public GameObject Instance { get; private set; }

    public void CreateSingleton(GameObject parent)
    {
        Instance ??= _instanceFactory(this, parent);
    }

    public void Destroy()
    {
        if (Instance is null)
            return;
        Object.Destroy(Instance);
        Instance = null;
    }
}
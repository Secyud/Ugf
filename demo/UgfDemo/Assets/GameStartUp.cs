using Demo;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.Prefab;
using UnityEngine;

public class GameStartUp : MonoBehaviour
{
    private IUgfApplication _application;
    
    private void Awake()
    {
        _application = UgfApplicationFactory.CreateAsync<DemoGameModule>().Result;
        _application.InitializeAsync().Wait();
        var manager = GetDependency<IPrefabControllerManager>();
        manager.Add<DemoPanelController>();
    }

    private T GetDependency<T>()
    {
        return _application.DependencyProvider.GetDependency<T>();
    }
}

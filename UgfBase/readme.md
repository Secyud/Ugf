- [Modularity](#modularity)
- [Dependency Injection](#dependency-injection)
- [DataManager](#datamanager)
- ...
# Modularity

* Provide a convenient way to control initialization sequence for different assembly.

```csharp
[DependsOn(
    typeof(UgfCoreModule)
)]
public class UgfUnityModule : IUgfModule
{
    public void ConfigureGame(ConfigurationContext context)
    {
        context.Manager.AddAssembly(typeof(UgfUnityModule).Assembly);
    }
}
```
* Class implement `IUgfModule` is regarded as a module. 
It provide a `Configure(..)` method to configure at the beginning of program.
* `IOnPreConfigure`, `IOnPostConfigure` can be used if you want to do some thing before or after all configuration.
* `IOnPreInitialization`, `IOnInitialization`, `IPostInitialization` is used for real game play.
* `IOnShutDown` provide a handling at end of game. Like return to main menu.

Module can be generated as plugin, so its also the base of plugin functions.

# Dependency Injection

>The Dependency Injection learned form ABP is changed a lot.
Different form web program, the dependency injection for game put emphasis on function management.
So the scope property is used for manage function dependency.

```csharp
public void Configure(ConfigurationContext context)
{
    context.Manager.AddTypes(
        typeof(DefaultLocalizerFactory),
        typeof(InputService));

    ...
}
```
* Service always registered in `Configure(..)`

```csharp
public void Configure(ConfigurationContext context)
{
    context.Manager.AddAssembly(
        typeof(UgfCoreModule).Assembly
        );

    ...
}
```
* Register assembly is also feasible.
>* AddAssembly will register all types of assembly so that data manager can use it configure data.
>* Simple functional module just need register service needed only.

```csharp
public ctor(DependService service)
{
    ..// use service;
}
```
* Service from Dependency Injection can use constructor injection to use service;
* Circular dependency is not allowed, use service form higher level or depend scope or depend module.

```csharp
var service = U.Get<DependService>();
..// use service;
```

* Dependency Injection auto handles services.
You don't need to known where it form, just use it.
* Service will be `null` if it is unavailable.

```csharp
var service = someScope.Get<DependService>();
..// use service;
```
* Scope is the class inherited from `DependencyScopeProvider`.
It provide another way to get service. Service always has a default scope.
Use `U.Get<>()` will return the default scope's service. Use `scope.Get<>()` if you need a new instance form the specific scope.
* The service should be scoped service. Singleton service will provide the global instance.

# DataManager

> Match the `Ugf.DataManager` tool. Provide data config, object generation functions.

# ...
using System;

namespace Secyud.Ugf.DependencyInjection
{
    public interface IDependencyProviderFactory:IDependencyScopeFactory,IDependencyProvider
    {
        DependencyDescriptor GetDescriptor(Type type);
    }
}
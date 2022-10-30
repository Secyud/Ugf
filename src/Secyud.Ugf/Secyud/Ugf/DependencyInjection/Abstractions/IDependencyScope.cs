using System;

namespace Secyud.Ugf.DependencyInjection;

public interface IDependencyScope : IDisposable
{
    public IDependencyProvider DependencyProvider { get; }
}
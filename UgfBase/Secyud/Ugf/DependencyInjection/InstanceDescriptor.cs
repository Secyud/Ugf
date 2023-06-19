using System;
using System.Diagnostics.CodeAnalysis;

namespace Secyud.Ugf.DependencyInjection;

public class InstanceDescriptor
{
    public Func<object> ObjectAccessor { get; set; }

    public object Instance { get; set; }

    public InstanceDescriptor(
        [NotNull] Func<object> objectAccessor)
    {
        ObjectAccessor = objectAccessor;
    }

    public InstanceDescriptor()
    {
    }
}
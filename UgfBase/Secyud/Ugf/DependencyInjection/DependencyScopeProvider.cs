#region

using System;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public class DependencyScopeProvider : DependencyProviderBase, IDisposable
    {
        public List<DependencyScopeProvider> SubProviders { get; } = new();
        public DependencyProviderBase ParentProvider { get; set; }

        public override DependencyDescriptor GetDependencyDescriptor(Type exposedType)
        {
            return ParentProvider.GetDependencyDescriptor(exposedType);
        }

        public virtual void OnInitialize()
        {
            
        }

        public virtual void Dispose()
        {
        }
    }
}
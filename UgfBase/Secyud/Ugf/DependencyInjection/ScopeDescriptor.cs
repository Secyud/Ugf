using System;
using System.Collections.Generic;

namespace Secyud.Ugf.DependencyInjection
{
    public class ScopeDescriptor
    {
        public DependencyScope Scope;
        public ScopeDescriptor ParentScope { get; }

        public List<ScopeDescriptor> SubScopes = new();

        public ScopeDescriptor(ScopeDescriptor parentScope)
        {
            ParentScope = parentScope;
        }
    }
}
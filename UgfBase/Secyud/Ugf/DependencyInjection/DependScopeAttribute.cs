using System;
using System.Diagnostics.CodeAnalysis;

namespace Secyud.Ugf.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class,Inherited = false)]
    public class DependScopeAttribute:Attribute
    {
        public Type DependScope { get; }

        public DependScopeAttribute([NotNull]Type dependScope)
        {
            DependScope = dependScope;
        }
    }
}
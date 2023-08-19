#region

using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections.Generic;
using Secyud.Ugf.Localization;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class GameInitializeContext : ModuleContextBase
    {
        public override IDependencyProvider Provider { get; }

        public GameInitializeContext(IDependencyProvider dependencyProvider)
        {
            Throw.IfNull(dependencyProvider);
            Provider = dependencyProvider;
        }
    }
}
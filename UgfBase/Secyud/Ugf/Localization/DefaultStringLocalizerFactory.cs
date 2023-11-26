#region

using Localization;
using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Ugf.Collections.Generic;

#endregion

namespace Secyud.Ugf.Localization
{
    public class DefaultStringLocalizerFactory : DefaultLocalizerFactory<string>
    {
        public DefaultStringLocalizerFactory(IDependencyRegistrar registrar) : base(registrar)
        {
        }
    }
}
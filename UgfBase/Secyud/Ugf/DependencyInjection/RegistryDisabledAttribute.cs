#region

using System;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegistryDisabledAttribute : Attribute
    {
    }
}
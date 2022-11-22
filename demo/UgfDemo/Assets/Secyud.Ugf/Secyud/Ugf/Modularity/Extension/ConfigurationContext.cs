using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public class ConfigurationContext
    {
        public ConfigurationContext(IDependencyManager manager)
        {
            Thrower.IfNull(manager);
            Manager = manager;
            Items = new Dictionary<string, object>();
        }

        public IDependencyManager Manager { get; }

        public IDictionary<string, object> Items { get; }


        public object this[string key]
        {
            get => Items.GetOrDefault(key);
            set => Items[key] = value;
        }
    }
}
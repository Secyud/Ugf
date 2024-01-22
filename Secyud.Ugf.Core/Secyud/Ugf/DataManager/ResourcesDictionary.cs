using System.Collections.Generic;

namespace Secyud.Ugf.DataManager
{
    public class ResourcesDictionary
    {
        private readonly SortedDictionary<int, ResourceDescriptor> _innerDictionary = new();
        
        public ResourceDescriptor Get(int id)
        {
            return _innerDictionary.GetValueOrDefault(id);
        }
        
        public void Add(ResourceDescriptor descriptor)
        {
            _innerDictionary[descriptor.Id] = descriptor;
        }
    }
}
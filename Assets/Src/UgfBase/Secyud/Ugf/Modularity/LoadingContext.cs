#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class LoadingContext : IReadOnlyDictionary<string, BinaryReader>, IDisposable
    {
        public LoadingContext(IDependencyProvider dependencyProvider)
        {
            Thrower.IfNull(dependencyProvider);
            DependencyProvider = dependencyProvider;
        }

        public IDependencyProvider DependencyProvider { get; }

        public T Get<T>() where T : class => DependencyProvider.Get<T>();

        private readonly Dictionary<string, BinaryReader> _readers = new();

        public BinaryReader AddLoadingReader(string name, string path)
        {
            BinaryReader reader = new(File.Open(path, FileMode.Open));
            _readers[name] = reader;
            return reader;
        }


        public IEnumerator<KeyValuePair<string, BinaryReader>> GetEnumerator()
        {
            return _readers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_readers).GetEnumerator();
        }

        public int Count => _readers.Count;

        public bool ContainsKey(string key)
        {
            return _readers.ContainsKey(key);
        }

        public bool TryGetValue(string key, out BinaryReader value)
        {
            return _readers.TryGetValue(key, out value);
        }

        public BinaryReader this[string key] => _readers[key];

        public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, BinaryReader>)_readers).Keys;

        public IEnumerable<BinaryReader> Values => ((IReadOnlyDictionary<string, BinaryReader>)_readers).Values;

        public void Dispose()
        {
            foreach (var reader in _readers.Values)
            {
                reader.Dispose();
            }
        }
    }
}
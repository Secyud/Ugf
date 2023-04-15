#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class SavingContext : IReadOnlyDictionary<string, BinaryWriter>, IDisposable
    {
        public SavingContext(IDependencyProvider dependencyProvider)
        {
            Thrower.IfNull(dependencyProvider);
            DependencyProvider = dependencyProvider;
        }

        public IDependencyProvider DependencyProvider { get; }

        public T Get<T>() where T : class => DependencyProvider.Get<T>();

        private readonly Dictionary<string, BinaryWriter> _writers = new();

        public BinaryWriter AddSavingWriter(string name, string path)
        {
            BinaryWriter writer = new(File.Open(path, FileMode.Create));
            _writers[name] = writer;
            return writer;
        }

        public IEnumerator<KeyValuePair<string, BinaryWriter>> GetEnumerator()
        {
            return _writers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_writers).GetEnumerator();
        }

        public int Count => _writers.Count;

        public bool ContainsKey(string key)
        {
            return _writers.ContainsKey(key);
        }

        public bool TryGetValue(string key, out BinaryWriter value)
        {
            return _writers.TryGetValue(key, out value);
        }

        public BinaryWriter this[string key] => _writers[key];

        public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, BinaryWriter>)_writers).Keys;

        public IEnumerable<BinaryWriter> Values => ((IReadOnlyDictionary<string, BinaryWriter>)_writers).Values;

        public void Dispose()
        {
            foreach (BinaryWriter writer in _writers.Values)
            {
                writer.Dispose();
            }
        }
    }
}
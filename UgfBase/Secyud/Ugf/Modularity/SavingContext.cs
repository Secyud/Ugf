#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class SavingContext : IReadOnlyDictionary<string, BinaryWriter>, IDisposable
    {
        private readonly Dictionary<string, BinaryWriter> _writers = new();
        private readonly ArchivingContext _context;

        public SavingContext(IDependencyProvider dependencyProvider)
        {
            Thrower.IfNull(dependencyProvider);
            _context = dependencyProvider.Get<ArchivingContext>();
            DependencyProvider = dependencyProvider;
        }

        public IDependencyProvider DependencyProvider { get; }

        public void Dispose()
        {
            foreach (var writer in _writers.Values) writer.Dispose();
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

        public T Get<T>() where T : class
        {
            return DependencyProvider.Get<T>();
        }

        public BinaryWriter AddWriter(string name)
        {
            string path = Path.Combine(Og.AppPath, $"/Archiving/{_context.CurrentSlot.Name}",name);
            BinaryWriter writer = new(File.Open(path, FileMode.Create));
            _writers[name] = writer;
            return writer;
        }
    }
}
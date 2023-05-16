#region

using Secyud.Ugf.Archiving;
using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#endregion

namespace Secyud.Ugf.Modularity
{
	public class SavingContext : IReadOnlyDictionary<string, BinaryWriter>, IDisposable
	{
		private readonly Dictionary<string, BinaryWriter> _writers = new();
		private readonly IArchivingContext _context;

		public SavingContext(IDependencyProvider dependencyProvider)
		{
			Thrower.IfNull(dependencyProvider);
			_context = dependencyProvider.Get<IArchivingContext>();
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

		public BinaryWriter GetWriter(string name)
		{
			var directory = Path.Combine(Og.ArchivingPath, _context.CurrentSlot.Id.ToString());
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			
			if (!_writers.TryGetValue(name, out var writer))
			{
				string path = Path.Combine(directory, name);
				
				writer = new BinaryWriter(File.Open(path, FileMode.Create));
				_writers[name] = writer;
			}

			return writer;
		}
	}
}
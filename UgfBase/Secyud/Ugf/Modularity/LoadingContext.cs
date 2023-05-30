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
	public class LoadingContext : IReadOnlyDictionary<string, BinaryReader>, IDisposable
	{
		private readonly Dictionary<string, BinaryReader> _readers = new();
		private readonly IArchivingContext _context;

		public LoadingContext(IDependencyProvider dependencyProvider)
		{
			Thrower.IfNull(dependencyProvider);
			_context = dependencyProvider.Get<IArchivingContext>();
		}

		public void Dispose()
		{
			foreach (BinaryReader reader in _readers.Values) reader.Dispose();
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

		public BinaryReader GetReader(string name)
		{
			if (!_readers.TryGetValue(name, out BinaryReader reader))
			{
				string path = Path.Combine(Og.ArchivingPath, _context.CurrentSlot.Id.ToString(), name);
				reader = new BinaryReader(File.Open(path, FileMode.Open));
				_readers[name] = reader;
			}
			return reader;
		}
	}
}
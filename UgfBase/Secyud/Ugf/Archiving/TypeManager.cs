#region

using Newtonsoft.Json;
using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Archiving
{
	public class TypeManager : Dictionary<Guid, TypeContainer>, ISingleton
	{
		private readonly MD5 _md5 = MD5.Create();
		private readonly Dictionary<string, Dictionary<string, Guid>> _idDictionary;

		public TypeManager()
		{
			string path = Path.Combine(Og.AppPath, "AssemblyInfo.json");

			if (!File.Exists(path))
			{
				_idDictionary = new Dictionary<string, Dictionary<string, Guid>>();
				return;
			}

			using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
			using StreamReader sr = new(fs, Encoding.UTF8);

			var jsonStr = sr.ReadToEnd();

			_idDictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Guid>>>(jsonStr);
		}

		public object Construct(Guid id) => this[id].Construct();

		public object Construct(BinaryReader reader) => this[reader.ReadGuid()].Construct();

		public T CloneInit<T>(T obj) where T : class
		{
			object ret =
				obj is ICloneable cloneable
					? cloneable.Clone()
					: Construct(obj.GetType());
			if (obj is ICopyable copyable)
				copyable.CopyTo(ret);
			return ret as T;
		}

		public object Construct(Type type)
		{
			Guid id = GetId(type);
			if (!TryGetValue(id, out var c))
			{
				c = new TypeContainer(type);
				this[id] = c;
			}
			if (c.Constructor is null)
				Debug.LogError($"As type with guid, {type} should have a non-parameter constructor but not!");
			return c.Construct();
		}

		public void TryAddType(Type type)
		{
			Guid id = GetId(type);
			if (TryGetValue(id, out TypeContainer origin))
				Debug.LogWarning($"Type manager: {type} replaced {origin.Type}");

			this[id] = new TypeContainer(type);
		}

		public Guid GetId(Type type)
		{
			if (_idDictionary.TryGetValue(type.AssemblyQualifiedName!, out Dictionary<string, Guid> dict))
			{
				if (!dict.TryGetValue(type.FullName!, out Guid id))
				{
					id = GenerateId(type);
					dict[type.FullName] = id;
				}

				return id;
			}
			else
			{
				dict = new Dictionary<string, Guid>();
				_idDictionary[type.AssemblyQualifiedName] = dict;
				Guid id = GenerateId(type);
				dict[type.FullName!] = id;
				return id;
			}
		}

		private Guid GenerateId(Type type)
		{
			string s = type.AssemblyQualifiedName + " " + type.FullName;
			Guid id = new(_md5.ComputeHash(Encoding.UTF8.GetBytes(s)));
			return id;
		}
	}
}
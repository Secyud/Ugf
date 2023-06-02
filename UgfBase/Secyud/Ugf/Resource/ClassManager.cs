#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Resource
{
	public class ClassManager : Dictionary<Guid, ClassDescriptor>, ISingleton
	{
		private readonly MD5 _md5 = MD5.Create();
		private readonly Dictionary<string, Dictionary<string, Guid>> _idDictionary;

		public ClassManager()
		{
			string path = Path.Combine(Og.AppPath,"Data", "AssemblyInfo.json");

			if (!File.Exists(path))
			{
				_idDictionary = new Dictionary<string, Dictionary<string, Guid>>();
				return;
			}

			using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
			using StreamReader sr = new(fs, Encoding.UTF8);

			string jsonStr = sr.ReadToEnd();

			_idDictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Guid>>>(jsonStr);
		}

		public object Construct(Guid id) => this[id].Construct();

		public object Construct(BinaryReader reader) => this[reader.ReadGuid()].Construct();

		public object ConstructSame(object obj) 
		{
			return Construct(obj.GetType());
		}

		public object Construct(Type type)
		{
			Guid id = GetId(type);
			if (!TryGetValue(id, out ClassDescriptor c))
			{
				c = new ClassDescriptor(type);
				this[id] = c;
			}
			if (c.Constructor is null)
				Debug.LogError($"As type with guid, {type} should have a non-parameter constructor but not!");
			object o = c.Construct();
			return o;
		}
		
		public void TryAddType(Type type)
		{
			Guid id = GetId(type);
			if (TryGetValue(id, out ClassDescriptor origin))
				Debug.LogWarning($"Type manager: {type} replaced {origin.Type}");

			this[id] = new ClassDescriptor(type);
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
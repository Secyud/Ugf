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
        private readonly Dictionary<string, Guid> _idDictionary;

        public ClassManager()
        {
            string path = Path.Combine(Og.AppPath, "Data", "AssemblyInfo.json");

            if (File.Exists(path))
            {
                using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
                using StreamReader sr = new(fs, Encoding.UTF8);

                string jsonStr = sr.ReadToEnd();

                _idDictionary = JsonConvert.DeserializeObject<Dictionary<string, Guid>>(jsonStr);
            }
            else
            {
                _idDictionary = new Dictionary<string, Guid>();
            }
        }

        public object Construct(Guid id)
        {
            if (TryGetValue(id, out ClassDescriptor descriptor))
                return descriptor.Construct();

            throw new UgfException($"Construct failed! cannot find class for Guid: {id}");
        }

        public TValue Construct<TValue>(Guid id)
        {
            object obj = Construct(id);
            if (obj is TValue ret)
                return ret;
            throw new UgfException(
                $"Construct failed! Class for Guid--{id} is a {obj.GetType()} but not a {typeof(TValue)}");
        }

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
            string fullName = type.FullName;

            if (fullName is null)
                throw new UgfException("Type full name is null, please check");

            if (!_idDictionary.TryGetValue(fullName, out Guid id))
            {
                id = GenerateId(fullName);
                _idDictionary[fullName] = id;
                return id;
            }

            return id;
        }

        private Guid GenerateId(string typeFullName)
        {
            return new Guid(_md5.ComputeHash(Encoding.UTF8.GetBytes(typeFullName)));
        }
    }
}
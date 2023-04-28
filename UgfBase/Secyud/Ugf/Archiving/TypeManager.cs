using System;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Archiving
{
    public class TypeManager : Dictionary<Guid, ConstructorContainer>, ISingleton
    {
        public object Construct(Guid id) => this[id].Construct();
        public object Construct(BinaryReader reader)=> this[reader.ReadGuid()].Construct();
        public object Construct(Type type) => this[type.GetId()].Construct();
        public T CloneInit<T>(T obj) where T : class => this[obj.GetTypeId()].Construct() as T;
    }
}
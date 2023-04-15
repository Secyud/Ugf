#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Archiving
{
    public class ArchivingManager : IScoped
    {
        private readonly IDependencyProvider _dependencyProvider;

        private readonly Dictionary<int, IArchivist> _archivists = new();

        public ArchivingManager(IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
        }

        public void InitializeArchivists(params Type[] archivists)
        {
            foreach (var archivist in archivists)
                InitializeArchivist(archivist);
        }

        public void InitializeArchivist(Type archivist)
        {
            if (_dependencyProvider.Get(archivist) is not IArchivist a)
                throw new Exception($"Type {archivist} is not a valid archivist!");

            _archivists[a.Id] = a;
        }

        public void Save(BinaryWriter writer, object obj)
        {
            var archivist = GetArchivist(obj.GetType());

            writer.Write(archivist.Id);

            archivist.Save(writer, obj);
        }

        public object Load(BinaryReader reader)
        {
            var id = reader.ReadInt32();
            return _archivists[id].Load(reader);
        }

        private IArchivist GetArchivist(MemberInfo type)
        {
            var attr = type.GetCustomAttribute<ArchivableAttribute>();

            if (attr is null)
                throw new Exception($"{type} does not have archivist!");

            return _dependencyProvider.Get(attr.ArchivistType) as IArchivist;
        }
    }
}
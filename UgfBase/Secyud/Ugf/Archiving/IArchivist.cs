#region

using System;
using System.IO;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Archiving
{
    public interface IArchivist<TObject> : ISingleton
    {
        int Id { get; }

        void Save(BinaryWriter writer, TObject obj);

        TObject Load(BinaryReader reader);
    }
}
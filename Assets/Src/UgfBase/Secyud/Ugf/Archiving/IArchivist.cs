#region

using System.IO;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Archiving
{
    public interface IArchivist : ISingleton
    {
        int Id { get; }

        void Save(BinaryWriter writer, object obj);

        object Load(BinaryReader reader);
    }
}
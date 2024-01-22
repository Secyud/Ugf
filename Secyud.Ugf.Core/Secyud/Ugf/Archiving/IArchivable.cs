#region

#endregion

using System.IO;

namespace Secyud.Ugf.Archiving
{
    public interface IArchivable
    {
        public void Save(BinaryWriter writer);

        public void Load(BinaryReader reader);
    }
}
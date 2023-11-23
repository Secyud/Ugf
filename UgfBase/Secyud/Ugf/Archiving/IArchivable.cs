#region

#endregion

namespace Secyud.Ugf.Archiving
{
    public interface IArchivable
    {
        public void Save(IArchiveWriter writer);

        public void Load(IArchiveReader reader);
    }
}
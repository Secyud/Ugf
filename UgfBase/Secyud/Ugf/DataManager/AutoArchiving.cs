using System;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class AutoArchiving:IArchivable,ICloneable
    {
        public virtual void Save(IArchiveWriter writer)
        {
            U.AutoSaveObject(this, writer);
        }

        public virtual void Load(IArchiveReader reader)
        {
            U.AutoLoadObject(this, reader);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
#region

using System;

#endregion

namespace Secyud.Ugf.Archiving
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ArchivableAttribute : Attribute
    {
        public readonly Type ArchivistType;

        public ArchivableAttribute(Type archivistType)
        {
            if (!typeof(IArchivist).IsAssignableFrom(archivistType))
                throw new UgfException($"{archivistType} is not a valid archivist.");

            ArchivistType = archivistType;
        }
    }
}
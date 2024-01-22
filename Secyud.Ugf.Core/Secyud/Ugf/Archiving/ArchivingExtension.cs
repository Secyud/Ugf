using System.Collections;
using System.IO;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.Modularity;

namespace Secyud.Ugf.Archiving
{
    public static class ArchivingExtension
    {
        public static void SaveResource(this IDataResource resource, BinaryWriter writer)
        {
            writer.Write(resource.ResourceId);
        }

        public static void LoadResource(this IDataResource shown, BinaryReader reader)
        {
            int resourceId = reader.ReadInt32();
            TypeManager.Instance
                .LoadObjectFromResource(shown, resourceId);
        }


        public static IEnumerator Loading(this IUgfApplication application)
        {
            foreach (IUgfModuleDescriptor module in application.Modules)
            {
                if (module.Instance is IOnArchiving initialization)
                {
                    yield return initialization.LoadGame();
                }
            }
        }

        public static IEnumerator Saving(this IUgfApplication applications)
        {
            foreach (IUgfModuleDescriptor descriptor in applications.Modules)
            {
                if (descriptor.Instance is IOnArchiving archiving)
                {
                    yield return archiving.SaveGame();
                }
            }
        }
    }
}
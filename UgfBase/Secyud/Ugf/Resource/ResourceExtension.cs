using System;

namespace Secyud.Ugf.Resource
{
    public static class ResourceExtension
    {
        public static ResourcedBase CreateAndInit(
            this string name,Type templateType)
        {
            ResourceDescriptor descriptor = Og.InitializeManager.GetResource(templateType, name);
            ResourcedBase resourcedBase = Og.ClassManager.Construct<ResourcedBase>(descriptor.TypeId);
            resourcedBase.Init(descriptor);
            return resourcedBase;
        }

        public static TResourcedBase CreateAndInit<TResourcedBase>(
            this string name) where TResourcedBase : ResourcedBase
        {
            var templateType = typeof(TResourcedBase);
            ResourceDescriptor descriptor = Og.InitializeManager.GetResource(templateType, name);
            TResourcedBase resourcedBase = Og.ClassManager.Construct<TResourcedBase>(descriptor.TypeId);
            resourcedBase.Init(descriptor);
            return resourcedBase;
        }

        public static TResourcedBase Init<TResourcedBase>(
            this TResourcedBase resourcedBase, ResourceDescriptor descriptor)
            where TResourcedBase : ResourcedBase
        {
            resourcedBase.InitSetting(descriptor);
            return resourcedBase;
        }
        public static TResourcedBase Init<TResourcedBase>(
            this TResourcedBase resourcedBase, string name)
            where TResourcedBase : ResourcedBase
        {
            ResourceDescriptor descriptor = Og.InitializeManager.GetResource(typeof(TResourcedBase), name);
            resourcedBase.InitSetting(descriptor);
            return resourcedBase;
        }
    }
}
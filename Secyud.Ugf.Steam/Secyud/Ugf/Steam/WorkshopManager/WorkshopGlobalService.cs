using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class WorkshopGlobalService : IRegistry
    {
        public List<WorkshopItemFilterTag> FilterTags { get; } = new();
    }
}
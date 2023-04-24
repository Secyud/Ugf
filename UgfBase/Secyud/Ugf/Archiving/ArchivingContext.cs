using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Archiving
{
    public class ArchivingContext:ISingleton
    {
        public readonly List<Slot> Slots = new();
        public Slot CurrentSlot;
    }
}
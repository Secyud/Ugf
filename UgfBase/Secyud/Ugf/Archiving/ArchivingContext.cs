#region

using Secyud.Ugf.DependencyInjection;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Archiving
{
	public class ArchivingContext : ISingleton
	{
		public readonly List<Slot> Slots = new();
		public Slot CurrentSlot;
	}
}
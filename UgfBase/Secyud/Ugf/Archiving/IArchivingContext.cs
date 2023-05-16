#region

using Secyud.Ugf.DependencyInjection;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Archiving
{
	public interface IArchivingContext : ISingleton
	{
		ISlot[] Slots { get; }

		ISlot CurrentSlot { get; set; }
		
		bool CurrentSlotExist { get; }
	}
}
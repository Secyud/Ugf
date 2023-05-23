#region

using Secyud.Ugf.DependencyInjection;

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
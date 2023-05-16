using Secyud.Ugf.Modularity;
using UnityEngine;

namespace Secyud.Ugf.Archiving
{
	public interface ISlot
	{
		int Id { get; }

		string Name { get;  }

		void PrepareSlotSaving(SavingContext context);
		void PrepareSlotLoading();

		void SetContent(RectTransform content);
	}
}
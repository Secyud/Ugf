#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public interface ITableProperty
	{
		public int Count { get; }

		public Transform CreateCell(Transform content, int index);

		public void ResetCell(Transform cell, int index);

		public void ApplyFilter();

		public void ApplySorter();
	}
}
#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.HexMap.Utilities
{
	/// <summary>
	///     Priority queue to store hex cells for the pathfinding algorithm.
	/// </summary>
	public class HexCellPriorityQueue
	{
		private readonly List<HexCell> _list = new();

		private int _minimum = int.MaxValue;

		/// <summary>
		///     How many cells are in the queue.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		///     Add a cell to the queue.
		/// </summary>
		/// <param name="cell">Cell to add.</param>
		public void Enqueue(HexCell cell)
		{
			Count += 1;
			int priority = cell.SearchPriority;
			if (priority < _minimum) _minimum = priority;

			while (priority >= _list.Count) _list.Add(null);

			cell.NextWithSamePriority = _list[priority];
			_list[priority] = cell;
		}

		/// <summary>
		///     Remove a cell from the queue.
		/// </summary>
		/// <returns>The cell with the highest priority.</returns>
		public HexCell Dequeue()
		{
			Count -= 1;
			for (; _minimum < _list.Count; _minimum++)
			{
				HexCell cell = _list[_minimum];
				if (cell != null)
				{
					_list[_minimum] = cell.NextWithSamePriority;
					return cell;
				}
			}

			return null;
		}

		/// <summary>
		///     Apply the currently priority of a cell that was previously enqueued.
		/// </summary>
		/// <param name="cell">Cell to update</param>
		/// <param name="oldPriority">Priority of the cell before it was changed.</param>
		public void Change(HexCell cell, int oldPriority)
		{
			HexCell current = _list[oldPriority];
			HexCell next = current.NextWithSamePriority;
			if (current == cell)
			{
				_list[oldPriority] = next;
			}
			else
			{
				while (next != cell)
				{
					current = next;
					next = current.NextWithSamePriority;
				}

				current.NextWithSamePriority = cell.NextWithSamePriority;
			}

			Enqueue(cell);
			Count -= 1;
		}

		/// <summary>
		///     Clear the queue.
		/// </summary>
		public void Clear()
		{
			_list.Clear();
			Count = 0;
			_minimum = int.MaxValue;
		}
	}
}
#region

using UnityEngine;

#endregion

namespace Secyud.Ugf
{
	public interface ICanBeShown
	{
		string ShowName { get; }

		string ShowDescription { get; }

		IObjectAccessor<Sprite> ShowIcon { get; }
	}
}
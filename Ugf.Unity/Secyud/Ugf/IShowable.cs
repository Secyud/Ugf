#region

using UnityEngine;

#endregion

namespace Secyud.Ugf
{
	public interface IShowable
	{
		string ShowName { get; }

		string ShowDescription { get; }

		IObjectAccessor<Sprite> ShowIcon { get; }
	}
}
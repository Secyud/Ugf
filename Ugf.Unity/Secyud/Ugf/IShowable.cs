#region

using UnityEngine;

#endregion

namespace Secyud.Ugf
{
	public interface IHasDescription
	{
		string ShowDescription { get; }
	}
	public interface IShowable:IHasDescription
	{
		string ShowName { get; }

		IObjectAccessor<Sprite> ShowIcon { get; }
	}
}
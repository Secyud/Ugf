#region

using Secyud.Ugf.ButtonComponents;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public sealed class FilterRegistrationGroup<TItem> : ICanBeEnabled
	{
		private bool _enabled = true;

		public List<FilterRegistration<TItem>> Filters { get; set; } = new();

		public string ShowName { get; set; }

		public string ShowDescription => null;

		public IObjectAccessor<Sprite> ShowIcon => null;

		public void SetEnabled(bool value)
		{
			_enabled = value;
		}

		public bool GetEnabled() => _enabled;
	}
}
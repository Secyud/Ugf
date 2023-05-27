#region

using Secyud.Ugf.DependencyInjection;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.ButtonComponents
{
	public abstract class ButtonFunctionBase<TItem> : ISingleton
	{
		private readonly List<ButtonRegistration<TItem>> _buttons = new();

		public IEnumerable<ButtonRegistration<TItem>> Get => _buttons;

		public void Register(ButtonRegistration<TItem> button)
		{
			_buttons.Add(button);
		}

		public void RegisterList(params ButtonRegistration<TItem>[] buttons)
		{
			foreach (ButtonRegistration<TItem> button in buttons) Register(button);
		}
	}
}
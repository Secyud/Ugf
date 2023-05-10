#region

using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.ButtonComponents;
using System.Linq;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public class Filter : MonoBehaviour
	{
		[SerializeField] private SText Name;
		[SerializeField] private SDualToggle Toggle;
		private FilterGroup _filterGroup;

		public void OnLeftClick()
		{
			Toggle.IsOn = !Toggle.IsOn;
			Refresh();
		}

		public void OnRightClick()
		{
			var value = _filterGroup.Filters.All(u => (this == u) ^ u.Toggle.IsOn);

			foreach (var filter in _filterGroup.Filters)
			{
				filter.Toggle.IsOn = (this == filter) ^ !value;
				filter.Refresh();
			}
		}

		private void Refresh()
		{
			_filterGroup.FunctionalTable.RefreshFilter();
		}

		private void OnInitialize(FilterGroup filterGroup, ICanBeEnabled canBeEnabled)
		{
			_filterGroup = filterGroup;
			Toggle.SetIsOnWithoutNotify(canBeEnabled.GetEnabled());
			Toggle.Bind(canBeEnabled.SetEnabled);
			Name.text = Og.L[canBeEnabled.ShowName];
		}

		public Filter Create(Transform parent, FilterGroup filterGroup, ICanBeEnabled canBeEnabled)
		{
			var ret = Instantiate(this, parent);

			ret.OnInitialize(filterGroup, canBeEnabled);

			return ret;
		}
	}
}
#region

using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.Localization;
using UnityEngine;

#endregion

namespace Secyud.Ugf.FunctionalComponents
{
	[RequireComponent(typeof(SImage))]
	public class SSpriteLocalizer : MonoBehaviour
	{
		[SerializeField] private string Value;

		private SImage _image;

		private void Awake()
		{
			_image = GetComponent<SImage>();
		}

		private void OnEnable()
		{
			_image.Sprite = DefaultLocalizer<Sprite>.Localizer[Value];
			enabled = false;
		}
	}
}
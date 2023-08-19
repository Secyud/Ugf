#region

using Secyud.Ugf.BasicComponents;
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
			_image.Sprite = U.S[Value];
			enabled = false;
		}
	}
}
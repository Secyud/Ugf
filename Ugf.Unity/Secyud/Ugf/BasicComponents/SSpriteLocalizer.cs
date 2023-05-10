#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.BasicComponents
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
			_image.Sprite = Og.IL[Value];
			enabled = false;
		}
	}
}
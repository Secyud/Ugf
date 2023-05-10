#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.BasicComponents
{
	[RequireComponent(typeof(SText))]
	public class SStringLocalizer : MonoBehaviour
	{
		[SerializeField] private string Value;
		[SerializeField] private bool Translate;

		private SText _text;

		private void Awake()
		{
			_text = GetComponent<SText>();
		}

		private void OnEnable()
		{
			_text.text = Translate ? Og.L.Translate(Value) : Og.L[Value];
			enabled = false;
		}
	}
}
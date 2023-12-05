#region

using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.Localization;
using UnityEngine;

#endregion

namespace Secyud.Ugf.FunctionalComponents
{
    [RequireComponent(typeof(SText))]
    public class SStringLocalizer : MonoBehaviour
    {
        [SerializeField] private bool Translate;
        private string _value;
        private SText _text;

        private void Awake()
        {
            _text = GetComponent<SText>();
            _value = _text.text;
        }

        private void OnEnable()
        {
            _text.text = Translate
                ? DefaultLocalizer<string>.Localizer.Translate(_value)
                : DefaultLocalizer<string>.Localizer[_value];
            enabled = false;
        }
    }
}
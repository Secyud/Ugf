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
        [SerializeField] private string Value;
        [SerializeField] private bool Translate;

        private SText _text;

        private void Awake()
        {
            _text = GetComponent<SText>();
        }

        private void OnEnable()
        {
            _text.text = Translate
                ? DefaultLocalizer<string>.Localizer.Translate(Value)
                : DefaultLocalizer<string>.Localizer[Value];
            enabled = false;
        }
    }
}
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class StringLocalizer : MonoBehaviour
    {
        private string _value;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _value = _text.text;
        }

        private void OnEnable()
        {
            _text.text = U.T[_value];
            enabled = false;
        }
    }
}
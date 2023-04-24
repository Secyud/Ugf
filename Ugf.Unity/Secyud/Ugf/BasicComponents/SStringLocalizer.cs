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

        private void Awake()
        {
            GetComponent<SText>().text =
                Translate ? Og.L.Translate(Value) : Og.L[Value];
        }
    }
}
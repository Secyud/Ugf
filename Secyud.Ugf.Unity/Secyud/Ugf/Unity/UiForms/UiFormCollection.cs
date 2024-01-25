using UnityEngine;

namespace Secyud.Ugf.Unity.UiForms
{
    public class UiFormCollection : MonoBehaviour
    {
        [field:SerializeField] public UiFormBase[] FormPrefabs { get; private set; }
    }
}
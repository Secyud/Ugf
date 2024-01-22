using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    public class MessageBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageText;

        public void Show(string message)
        {
            gameObject.SetActive(true);
            _messageText.text = message;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
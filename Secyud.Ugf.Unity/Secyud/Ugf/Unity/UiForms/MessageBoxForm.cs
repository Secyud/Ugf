using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Unity.UiForms
{
    public class MessageBoxForm : UiFormBase<MessageBoxForm>
    {
        [SerializeField] private TextMeshProUGUI _messageText;

        public static void ShowMessage(string message)
        {
            ActionWithPrefab(Show);
            return;

            void Show(UiFormGroup.Element element)
            {
                element.CreateInstance();
                element.ShowForm();
                MessageBoxForm messageBox = (MessageBoxForm)element.Instance;
                messageBox.Show(message);
            }
        }
        
        public void Show(string message)
        {
            ShowForm();
            _messageText.text = message;
        }

        public void Hide()
        {
            HideForm();
        }
    }
}
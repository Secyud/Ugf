using System;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Unity.UiForms
{
    public class ConfirmationForm : UiFormBase<ConfirmationForm>
    {
        [SerializeField] private TextMeshProUGUI _messageText;

        private Action _callback;

        public override int GroupId => 2048;

        public static void ShowConfirmation(string message, Action callback)
        {
            ActionWithPrefab(Show);
            return;

            void Show(UiFormGroup.Element element)
            {
                element.CreateInstance();
                element.ShowForm();
                ConfirmationForm confirmation = (ConfirmationForm)element.Instance;
                confirmation.Show(message, callback);
            }
        }

        public void Show(string message, Action callback)
        {
            ShowForm();
            _callback = callback;
            _messageText.text = message;
        }

        public void Hide()
        {
            _callback = null;
            HideForm();
        }

        public void Ensure()
        {
            _callback?.Invoke();
            Hide();
        }
    }
}
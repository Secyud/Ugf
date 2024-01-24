using System;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Game.DialogueManager
{
    /// <summary>
    /// Selection of dialogue branch.
    /// The action always set to different branch.
    /// </summary>
    public class DialogueSelection : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _selectText;
        [SerializeField] private TextMeshProUGUI _selectIndex;
        [SerializeField] private int _selectIndexNum;

        private Action _action;

        public int SelectIndex
        {
            get => _selectIndexNum;
            set
            {
                _selectIndexNum = value;
                _selectIndex.text = $"({_selectIndexNum})";
            }
        }

        public void OnClick()
        {
            _action?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha0 + _selectIndexNum))
            {
                OnClick();
            }
        }

        public void SetActiveOption(string text, Action action)
        {
            _selectText.text = text;
            _action = action;
            gameObject.SetActive(true);
        }
    }
}
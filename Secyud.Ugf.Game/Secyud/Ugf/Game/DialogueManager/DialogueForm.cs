#region

using System;
using System.Collections.Generic;
using System.Ugf.Collections.Generic;
using Secyud.Ugf.Unity.Ui;
using Secyud.Ugf.Unity.UiForms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.Game.DialogueManager
{
    /// <summary>
    /// A dialogue form
    /// </summary>
    public class DialogueForm : UiFormBase<DialogueForm>
    {
        [SerializeField] private DialogueSelection _selectionTemplate;
        [SerializeField] private Image _backGround;
        [SerializeField] private Image _roleAvatar;
        [SerializeField] private TextMeshProUGUI _roleNameField;
        [SerializeField] private TextMeshProUGUI _textField;
        [SerializeField] private LayoutTrigger _selectContent;
        [SerializeField] private Button _continueButton;
        [SerializeField] private int _maxSelectOptionCount = 9;
        [SerializeField] private bool _allowKeyInput;
        [SerializeField] private DialogueSelection[] _selections;

        private DialogueService _dialogueService;
        private IList<ActionWithText> _selectionUnits;
        private int _currentPage;

        protected virtual void Awake()
        {
            _dialogueService = U.Get<DialogueService>();
            _continueButton.onClick.AddListener(ContinueDialogue);
            if (_maxSelectOptionCount <= 0)
            {
                _maxSelectOptionCount = _selections.Length;
            }
            else
            {
                _selections = new DialogueSelection[_maxSelectOptionCount];
                for (int i = 0; i < _maxSelectOptionCount; i++)
                {
                    DialogueSelection optionItem = Instantiate(
                        _selectionTemplate, _selectContent.RectTransform);
                    optionItem.SelectIndex = i + 1;
                    _selections[i] = optionItem;
                }
            }
        }

        protected virtual void Update()
        {
            if (!_allowKeyInput)
            {
                enabled = false;
                return;
            }

            if (Input.GetKey(KeyCode.Space) &&
                _continueButton.gameObject.activeSelf)
            {
                ContinueDialogue();
            }
        }

        public virtual void SetDialogue(DialogueUnit dialogue)
        {
            _textField.text = U.T[dialogue.DialogueText];
            if (dialogue.ShowRoleNameAndAvatar)
            {
                _roleNameField.text = U.T[dialogue.RoleName];
                dialogue.Avatar?.GetValueAsync(u => _roleAvatar.sprite = u);
            }
            else
            {
                _roleNameField.enabled = false;
                _roleAvatar.enabled = false;
            }

            if (dialogue.ShowBackgroundImage)
            {
                dialogue.Background.GetValueAsync(u =>
                {
                    if (u)
                    {
                        _backGround.sprite = u;
                    }
                });
            }
            else
            {
                _backGround.enabled = false;
            }

            _continueButton.gameObject.SetActive(dialogue.ShowContinueButton);

            if (dialogue.ShowSelectOptions &&
                !dialogue.SelectOptions.IsNullOrEmpty())
            {
                _selectionUnits = dialogue.SelectOptions;
                _currentPage = 0;
                UpdateSelectContent();
                _selectContent.gameObject.SetActive(true);
            }
            else
            {
                _selectContent.gameObject.SetActive(false);
            }
        }

        protected virtual void ContinueDialogue()
        {
            _dialogueService.ContinueDialogue();
        }

        protected virtual void NextSelectionPage()
        {
            _currentPage++;
            UpdateSelectContent();
        }

        protected virtual void PreviousSelectionPage()
        {
            _currentPage--;
            UpdateSelectContent();
        }

        protected virtual void UpdateSelectContent()
        {
            int startIndex = (_maxSelectOptionCount - 2) * _currentPage;
            int itemCount = _maxSelectOptionCount - 2;
            if (_currentPage > 0) startIndex++;
            else itemCount++;
            if (startIndex + itemCount >= _selectionUnits.Count - 1) itemCount++;
            itemCount = Math.Min(_selectionUnits.Count - startIndex, itemCount);

            int itemIndex = 0;
            for (; itemIndex < itemCount; itemIndex++)
            {
                ActionWithText unit = _selectionUnits[startIndex + itemIndex];
                _selections[itemIndex].SetActiveOption(unit.Text, unit.Action);
            }

            if (_currentPage > 0)
            {
                _selections[itemIndex++].SetActiveOption(
                    U.T["PreviousPage"], PreviousSelectionPage);
            }

            if (startIndex + itemCount < _selectionUnits.Count)
            {
                _selections[itemIndex++].SetActiveOption(
                    U.T["NextPage"], NextSelectionPage);
            }

            for (; itemIndex < _maxSelectOptionCount; itemCount++)
            {
                _selections[itemIndex++].gameObject.SetActive(false);
            }

            _selectContent.Refresh();
        }
    }
}
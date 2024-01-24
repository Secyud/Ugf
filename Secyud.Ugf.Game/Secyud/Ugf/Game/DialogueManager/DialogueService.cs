using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Unity.AssetLoading;
using UnityEngine;

namespace Secyud.Ugf.Game.DialogueManager
{
    public class DialogueService : IRegistry
    {
        public PrefabContainer<DialoguePanel> DialoguePanelPrefab { get; set; }

        private DialoguePanel _dialoguePanelInstance;
        private IList<DialogueUnit> _dialogues;
        private int _currentDialogueIndex;

        /// <summary>
        /// Set a new dialogue, the dialogue Instantiated on
        /// the main canvas. You can call this method multi
        /// times if you want to set branch in dialogue. 
        /// </summary>
        /// <param name="dialogues">The dialogue list you want to set.</param>
        /// <exception cref="UgfNotRegisteredException"></exception>
        public void OpenDialoguePanel(IList<DialogueUnit> dialogues)
        {
            // Instance is exist, move it to the front of UIs.
            if (_dialoguePanelInstance)
            {
                Transform transform = _dialoguePanelInstance.transform;
                transform.SetSiblingIndex(transform.parent.childCount - 1);
                return;
            }

            if (DialoguePanelPrefab is null)
            {
                throw new UgfNotRegisteredException(
                    nameof(DialogueService),
                    nameof(DialoguePanelPrefab));
            }

            _dialogues = dialogues;
            _currentDialogueIndex = 0;

            DialoguePanelPrefab.GetValueAsync(p =>
                _dialoguePanelInstance = p.Instantiate(U.Canvas.transform));
        }


        internal void ContinueDialogue()
        {
            _currentDialogueIndex++;
            if (_currentDialogueIndex < _dialogues.Count)
            {
                _dialoguePanelInstance.SetDialogue(
                    _dialogues[_currentDialogueIndex]);
            }
            else
            {
                CloseDialoguePanel();
            }
        }

        public void CloseDialoguePanel()
        {
            _dialoguePanelInstance.Destroy();
        }
    }
}
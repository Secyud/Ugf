using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Unity.AssetLoading;
using UnityEngine;

namespace Secyud.Ugf.Game.DialogueManager
{
    public class DialogueService : IRegistry
    {
        public MonoContainer<DialoguePanel> DialoguePanel { get;  } 
            = MonoContainer<DialoguePanel>.OnCanvas();

        private IList<DialogueUnit> _dialogues;
        private int _currentDialogueIndex;

        /// <summary>
        /// Set a new dialogue, the dialogue Instantiated on
        /// the main canvas. You can call this method multi
        /// times if you want to set branch in dialogue. 
        /// </summary>
        /// <param name="dialogues">The dialogue list you want to set.</param>
        public void OpenDialoguePanel(IList<DialogueUnit> dialogues)
        {
            _dialogues = dialogues;
            _currentDialogueIndex = 0;

            DialoguePanel.GetValueAsync(p =>
            {
                p.SetDialogue(_dialogues[0]);
            });
        }

        internal void ContinueDialogue()
        {
            _currentDialogueIndex++;
            if (_currentDialogueIndex < _dialogues.Count)
            {
                DialoguePanel.GetValue().SetDialogue(
                    _dialogues[_currentDialogueIndex]);
            }
            else
            {
                CloseDialoguePanel();
            }
        }

        public void CloseDialoguePanel()
        {
            DialoguePanel.Destroy();
        }
    }
}
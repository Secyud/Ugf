using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Game.DialogueManager
{
    public class DialogueService : IRegistry
    {
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
            if (dialogues.IsNullOrEmpty())
            {
                return;
            }

            _dialogues = dialogues;
            _currentDialogueIndex = 0;

            DialogueForm.CreateForm();
            DialogueForm.ShowForm();
            DialogueForm.GetForm().SetDialogue(_dialogues[0]);
        }

        internal void ContinueDialogue()
        {
            _dialogues[_currentDialogueIndex].DefaultAction?.Invoke();
            _currentDialogueIndex++;
            if (_currentDialogueIndex < _dialogues.Count)
            {
                DialogueForm.GetForm().SetDialogue(
                    _dialogues[_currentDialogueIndex]);
            }
            else
            {
                CloseDialoguePanel();
            }
        }

        public void CloseDialoguePanel()
        {
            DialogueForm.DestroyFrom();
        }
    }
}
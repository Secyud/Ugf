using System;

namespace Secyud.Ugf.Game.DialogueManager
{
    /// <summary>
    /// Describe a button or trigger.
    /// It only has Text and Action.
    /// </summary>
    public class ActionWithText
    {
        public string Text { get; set; }
        public Action Action { get; set; }
    }
}
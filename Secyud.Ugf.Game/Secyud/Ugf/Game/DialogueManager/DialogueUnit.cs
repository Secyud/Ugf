﻿using System;
using System.Collections.Generic;
using Secyud.Ugf.Unity.AssetLoading;
using UnityEngine;

namespace Secyud.Ugf.Game.DialogueManager
{
    public class DialogueUnit
    {
        public string DialogueText { get; set; }
        public string RoleName { get; set; }
        public IObjectContainer<Sprite> Avatar { get; set; }
        public IObjectContainer<Sprite> Background { get; set; }
        public IList<ActionWithText> SelectOptions { get; set; }
        public bool ShowSelectOptions { get; set; }
        public bool ShowBackgroundImage { get; set; }
        public bool ShowRoleNameAndAvatar { get; set; }
        public bool ShowContinueButton { get; set; }
        public Action DefaultAction { get; set; }
    }
}
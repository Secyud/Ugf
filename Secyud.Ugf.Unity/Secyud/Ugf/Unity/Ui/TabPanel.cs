﻿using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    public class TabPanel : MonoBehaviour
    {
        public TabGroup Group { get; internal set; }

        public void SelectThisTab()
        {
            Group.SelectTab(this);
        }

        public virtual void OnShowing()
        {
        }

        public virtual void OnHiding()
        {
        }
    }
}
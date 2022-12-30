using System;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    public class BasicPager : MonoBehaviour, IListPager
    {
        public Button FirstPage;
        public Button PrePage;
        public Button NextPage;
        public Button LastPage;

        public Action<IListPager> PageAction { get; set; }
        
        public int MaxPage { get; set; }

        public int Page { get; set; } = 0;

        public int Count { get; set; } = 10;

        private void Awake()
        {
            FirstPage.onClick.AddListener(() => TurnToPage(0));
            PrePage.onClick.AddListener(() => TurnToPage(Page - 1));
            NextPage.onClick.AddListener(() => TurnToPage(Page + 1));
            LastPage.onClick.AddListener(() => TurnToPage(MaxPage - 1));
        }

        private void TurnToPage(int page)
        {
            if (page >= MaxPage)
                page = MaxPage - 1;
            else if (page < 0)
                page = 0;

            Page = page;

            PageAction?.Invoke(this);

            CheckButtonState();
        }

        public void CheckButtonState()
        {
            if (Page <= 0)
            {
                FirstPage.enabled = false;
                PrePage.enabled = false;
            }
            else
            {
                FirstPage.enabled = true;
                PrePage.enabled = true;
            }

            if (Page >= MaxPage - 1)
            {
                NextPage.enabled = false;
                LastPage.enabled = false;
            }
            else
            {
                NextPage.enabled = true;
                LastPage.enabled = true;
            }
        }
    }
}
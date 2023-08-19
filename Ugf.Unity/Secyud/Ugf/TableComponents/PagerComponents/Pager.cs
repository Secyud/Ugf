using Secyud.Ugf.BasicComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.PagerComponents
{
    public sealed class Pager : TableComponentBase<Pager, PagerDelegate>
    {
        public override string Name => nameof(Pager);
        [SerializeField] private int ItemPerPage = 12;
        [SerializeField] private SText PageText;

        private int _page = 1;

        private int _refreshLevel;

        public int MaxPage => (Delegate.Count + PageSize - 1) / PageSize;

        public int PageSize => ItemPerPage;

        public Table Table { private get; set; }

        public void RefreshPage()
        {
            Table.Refresh();
        }

        public int Page
        {
            get => _page;
            private set
            {
                if (MaxPage == 0 || value < 1)
                    _page = 1;
                else if (value > MaxPage)
                    _page = MaxPage;
                else
                    _page = value;
                int indexFirst = (_page - 1) * PageSize;
                Table.Delegate.IndexFirst = indexFirst;
                Table.Delegate.IndexLast = indexFirst + PageSize;
                PageText.text = _page.ToString();
                RefreshPage();
            }
        }

        public void TurnToFirstPage()
        {
            Page = 1;
        }

        public void TurnToPreviewPage()
        {
            Page -= 1;
        }

        public void TurnToNextPage()
        {
            Page += 1;
        }

        public void TurnToLastPage()
        {
            Page = MaxPage;
        }
    }
}
namespace Secyud.Ugf.Unity.Ui
{
    public class ListPagerAndSorter:ListSorter,IListPager
    {
        public int Page { get; set; }
        public int Count { get; set; }
    }
}
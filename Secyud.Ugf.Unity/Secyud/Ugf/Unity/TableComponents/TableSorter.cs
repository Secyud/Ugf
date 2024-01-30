namespace Secyud.Ugf.Unity.TableComponents
{
    public abstract class TableSorter:TableOperator
    {
        protected TableFilter Filter;

        protected virtual void Awake()
        {
            Filter = GetComponent<TableFilter>();
        }
    }
}
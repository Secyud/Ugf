namespace Secyud.Ugf.Unity.TableComponents
{
    public abstract class TableFilter:TableOperator
    {
        protected TableSource Source;

        protected virtual void Awake()
        {
            Source = GetComponent<TableSource>();
        }
    }
}
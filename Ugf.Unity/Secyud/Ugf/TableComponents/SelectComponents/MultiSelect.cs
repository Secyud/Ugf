namespace Secyud.Ugf.TableComponents.SelectComponents
{
    public class MultiSelect: TableComponentBase<MultiSelect, MultiSelectDelegate>
    {
        public override string Name => nameof(MultiSelect);
        
        public void OnEnsure()
        {
            Delegate.OnEnsure();
        }
    }
}
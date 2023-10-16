namespace Secyud.Ugf.HexMap
{
    public abstract class CellProperty
    {
        public HexCell Cell { get; private set; }

        public TProperty Get<TProperty>()
            where TProperty : CellProperty 
            => Cell.Get<TProperty>();
        public virtual bool SaveProperty => true;
        public virtual void Initialize(HexCell cell)
        {
            Cell = cell;
        }
    }
}
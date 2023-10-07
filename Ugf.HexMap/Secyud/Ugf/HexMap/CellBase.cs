namespace Secyud.Ugf.HexMap
{
    public abstract class CellBase
    {
        public abstract void SetHighlight();
        
        public HexCell Cell { get; set; }

        public virtual void Bind(HexCell cell)
        {
            Cell = cell;
            cell.Message = this;
        }
    }
}
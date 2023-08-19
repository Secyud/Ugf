namespace Secyud.Ugf.HexMap
{
    public abstract class CellBase
    {
        public abstract void SetHighlight();
        
        public HexCell Cell { get; internal set; }
    }
}
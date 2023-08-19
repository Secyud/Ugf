namespace Secyud.Ugf.HexMap
{
    public interface IUnitBase
    {
        public HexUnit Unit { get; set; }

        void OnDying();

        void OnEndPlay();
    }
}
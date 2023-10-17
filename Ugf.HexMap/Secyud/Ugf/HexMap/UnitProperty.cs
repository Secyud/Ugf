namespace Secyud.Ugf.HexMap
{
    public abstract class UnitProperty
    {
        public HexUnit Unit { get; private set; }
        
        public TProperty Get<TProperty>()
            where TProperty : UnitProperty 
            => Unit.Get<TProperty>();
        public virtual void Initialize(HexUnit unit)
        {
            Unit = unit;
        }
    }
}
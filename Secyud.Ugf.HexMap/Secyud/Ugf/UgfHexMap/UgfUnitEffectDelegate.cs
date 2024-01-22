namespace Secyud.Ugf.UgfHexMap
{
    public class UgfUnitEffectDelegate
    {
        
        public virtual void OnInitialize(UgfUnitEffect effect, UgfUnit unit, UgfCell target)
        {
            effect.SetDelegate(this,unit,target);
        }

        public virtual void OnUpdate(UgfUnitEffect effect)
        {
            
        }

        public virtual void OnDestroy(UgfUnitEffect effect)
        {
            effect.Unit.OnPlayFinished();
        }
    }
}
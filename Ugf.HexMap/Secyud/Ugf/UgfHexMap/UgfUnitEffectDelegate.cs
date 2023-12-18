using Secyud.Ugf.HexMap;
using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public class UgfUnitEffectDelegate
    {
        protected UgfUnit Unit { get; set; }
        protected UgfUnitEffect Effect { get; set; }
        
        public virtual void OnInitialize(UgfUnitEffect effect, UgfUnit unit, HexCell target)
        {
            Effect = effect;
            Unit = unit;
            Effect.Delegate = this;
        }

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnDestroy()
        {
            Unit.OnPlayFinished();
        }
    }
}
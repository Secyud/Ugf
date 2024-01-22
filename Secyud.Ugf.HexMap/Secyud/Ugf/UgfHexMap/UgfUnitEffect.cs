using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public class UgfUnitEffect : MonoBehaviour
    {
        private UgfUnitEffectDelegate _effectDelegate;

        public UgfUnit Unit { get; private set; }
        public UgfCell Target { get; private set; }

        public virtual void SetDelegate(UgfUnitEffectDelegate effectDelegate,
            UgfUnit unit, UgfCell target)
        {
            _effectDelegate = effectDelegate;
            Unit = unit;
            Target = target;
        }

        protected virtual void Update()
        {
            _effectDelegate?.OnUpdate(this);
        }

        protected virtual void OnDestroy()
        {
            _effectDelegate?.OnDestroy(this);
        }
    }
}
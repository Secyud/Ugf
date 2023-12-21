using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    [RequireComponent(typeof(ParticleSystem))]
    public class UgfUnitEffect : MonoBehaviour
    {
        private UgfUnitEffectDelegate _effectDelegate;

        public UgfUnit Unit { get; private set; }
        public UgfCell Target { get; private set; }
        public ParticleSystem Particle { get; private set; }

        public virtual void SetDelegate(UgfUnitEffectDelegate effectDelegate,
            UgfUnit unit, UgfCell target)
        {
            _effectDelegate = effectDelegate;
            Unit = unit;
            Target = target;
        }

        protected void Awake()
        {
            Particle = GetComponent<ParticleSystem>();
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
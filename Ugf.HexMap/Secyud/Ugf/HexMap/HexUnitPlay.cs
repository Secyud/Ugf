using UnityEngine;

namespace Secyud.Ugf.HexMap
{
    public abstract class HexUnitPlay : MonoBehaviour
    {
        [SerializeField] private float PlayTime;
        [SerializeField] public bool Loop;
        protected HexUnit Unit;
        protected float LastTime;
        protected HexCell TargetCell;

        protected virtual void Awake()
        {
            enabled = false;
            LastTime = PlayTime;
        }

        protected virtual void Update()
        {
            LastTime -= Time.deltaTime;
            if (LastTime < 0)
                EndPlay();
            else
                OnUpdate();
        }

        protected abstract void OnUpdate();

        public virtual void Play(HexUnit unit,HexCell targetCell)
        {
            Unit = unit;
            TargetCell = targetCell;
            Transform trans = transform;
            trans.parent = unit.transform;
            trans.localPosition = Vector3.zero;
            enabled = !Loop;
            ContinuePlay(unit);
        }

        public virtual void ContinuePlay(HexUnit unit)
        {
            
        }

        protected virtual void EndPlay()
        {
            Unit.OnPlayFinished();
            Destroy(this);
        }
    }
}
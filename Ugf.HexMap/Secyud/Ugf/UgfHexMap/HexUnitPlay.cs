using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public abstract class HexUnitPlay : MonoBehaviour
    {
        [SerializeField] protected float PlayTime;
        [SerializeField] protected bool Loop;
        [SerializeField] protected bool OnUnit;
        protected UgfUnit Unit;
        protected float LastTime;
        protected UgfCell TargetCell;

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

        public virtual void Play(UgfUnit unit,UgfCell targetCell)
        {
            Unit = unit;
            TargetCell = targetCell;
            Transform trans = transform;
            if (OnUnit)
            {
                trans.parent = unit.Unit.transform;
            }
            trans.position = unit.Unit.transform.position;
            trans.LookAt(targetCell.Position,Vector3.up);
            enabled = !Loop;
            ContinuePlay(unit);
        }

        protected virtual void ContinuePlay(UgfUnit unit)
        {
            
        }

        protected virtual void EndPlay()
        {
            Unit.OnPlayFinished();
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

namespace Secyud.Ugf.HexMap
{
    public abstract class HexUnitPlay : MonoBehaviour
    {
        [SerializeField] private float PlayTime;
        [SerializeField] public bool Loop;
        private HexUnit _unit;
        private float _lastTime;

        protected virtual void Awake()
        {
            enabled = false;
            _lastTime = PlayTime;
        }

        protected virtual  void Update()
        {
            _lastTime -= PlayTime;
            if (_lastTime < 0)
                EndPlay();
        }

        public virtual void Play(HexUnit unit)
        {
            _unit = unit;
            Transform trans = transform;
            trans.parent = unit.transform;
            trans.localPosition = Vector3.zero;
            enabled = !Loop;
            ContinuePlay(unit);
        }

        public abstract void ContinuePlay(HexUnit unit);

        protected virtual void EndPlay()
        {
            _unit.OnPlayFinished();
            Destroy(this);
        }
    }
}
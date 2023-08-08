using System;
using UnityEngine;

namespace Secyud.Ugf.BasicComponents
{
    public class SPopupQueue:MonoBehaviour
    {
        [SerializeField] private RectTransform Content;
        [SerializeField] private float Duration = 2;
        [SerializeField] private float MoveSpeed = 5;
        [SerializeField] private int MaxCount = 5;
        [SerializeField] private bool Transparency;

        private Tuple<RectTransform,float>[] _transforms;
        private RectTransform _current;
        private int _currentPos;
        private float _timeRecord;
        private bool _move;

        public void Awake()
        {
            _transforms = new Tuple<RectTransform,float>[MaxCount];
        }

        public void Update()
        {
            _timeRecord += Time.deltaTime;
            
            for (int i = 0; i < MaxCount; i++)
            {
                var tuple = _transforms[i];
                if (tuple is not null &&
                    tuple.Item2 <= _timeRecord)
                {
                    tuple.Item1.Destroy();
                    _transforms[i] = null;
                }
            }

            if (_move)
            {
                var trans = _transforms[_currentPos].Item1;
                //trans.localPosition += 
                for (int i = 0; i < MaxCount; i++)
                {
                    var tuple = _transforms[i];
                    if (tuple is not null &&
                        tuple.Item2 <= _timeRecord)
                    {
                        tuple.Item1.Destroy();
                        _transforms[i] = null;
                    }
                }
            }
        }
    }
}
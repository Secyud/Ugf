using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.Unity.Ui
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private UnityEvent _timeOutEvent;
        [SerializeField] private float _time;
        public UnityEvent OnTimeOut => _timeOutEvent;

        private void Update()
        {
            _time -= Time.deltaTime;
            if (_time < 0)
            {
                _timeOutEvent.Invoke();
                Destroy(this);
            }
        }
    }
}
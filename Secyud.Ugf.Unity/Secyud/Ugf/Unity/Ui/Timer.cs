using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.Unity.Ui
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float _time;
        [field: SerializeField] public UnityEvent OnTimeOut { get; private set; }

        private void Update()
        {
            _time -= Time.deltaTime;
            if (_time < 0)
            {
                OnTimeOut.Invoke();
                Destroy(this);
            }
        }
    }
}
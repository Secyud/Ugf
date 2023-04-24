#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.FunctionalComponents
{
    public class DestroyOnTimeout : MonoBehaviour
    {
        private static DestroyOnTimeout _instance;
        [SerializeField] private float OutTime;
        private float _timeRecord;

        private void Awake()
        {
            if (_instance)
                Destroy(_instance.gameObject);
            _instance = this;
        }

        private void Update()
        {
            _timeRecord += Time.deltaTime;
            if (OutTime < _timeRecord)
                Destroy(gameObject);
        }
    }
}
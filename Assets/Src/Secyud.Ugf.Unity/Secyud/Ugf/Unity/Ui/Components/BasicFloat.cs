using System;
using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    [RequireComponent(typeof(RectTransform))]
    public class BasicFloat : MonoBehaviour
    {
        private float _timeRecord;
        protected RectTransform RectTransform;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            _timeRecord += Time.deltaTime;
            if (_timeRecord < 0.2) return;

            CheckPosition();
            CheckMouse();
        }

        protected virtual void CheckPosition()
        {
            var x = Math.Min(Input.mousePosition.x, Screen.width - (RectTransform.offsetMax.x - RectTransform.offsetMin.x));
            var y = Math.Min(Input.mousePosition.y, Screen.height - (RectTransform.offsetMax.y - RectTransform.offsetMin.y));

            RectTransform.localPosition = new Vector3(x, 0, y);
        } 
        
        protected virtual void CheckMouse()
        {
            
        } 
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.Unity.EditorComponents
{
    /// <summary>
    /// 属性设定时应当有所反应， 以确认属性设定的真实值，
    /// 真实值可能用于文字显示，图形变化，一般加入
    /// SetWithoutNotification之类的事件
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class EditorEvent<TValue>
    {
        [SerializeField] private UnityEvent<TValue> _event = new();

        public void Invoke(TValue value)
        {
            _event.Invoke(value);
        }
    }
}
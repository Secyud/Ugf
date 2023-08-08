using System;
using System.Numerics;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.BasicComponents
{
    [Serializable]
    public class VoidEvent : UnityEvent
    {
    }
    [Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }
    [Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }
    [Serializable]
    public class FloatEvent : UnityEvent<float>
    {
    }
    [Serializable]
    public class Vector2Event : UnityEvent<Vector2>
    {
    }
    [Serializable]
    public class PointerEvent : UnityEvent<PointerEventData>
    {
    }
}
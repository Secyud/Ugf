using Secyud.Ugf.Abstraction;
using UnityEngine;

namespace Secyud.Ugf.Unity.InputManagement
{
    public interface IInputEvent : IHasName,IHasId<int>
    {
        KeyCode KeyCode { get; set; }
        FunctionKey FunctionKey { get; set; }
        void Invoke();
    }
}
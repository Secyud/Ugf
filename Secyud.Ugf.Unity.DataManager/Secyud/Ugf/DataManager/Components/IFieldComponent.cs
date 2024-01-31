using UnityEngine;

namespace Secyud.Ugf.DataManager.Components
{
    public interface IFieldComponent
    {
        void SetVisibility(bool visibility, bool root = false);
        Transform Last { get; }
        void Die();
    }
}
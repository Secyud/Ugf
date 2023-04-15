#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public interface ITableProperty
    {
        public int ItemsCount { get; }
        public Transform SetCell(Transform content, int index);
        public void ApplyFilter();
        public void ApplySorter();
    }
}
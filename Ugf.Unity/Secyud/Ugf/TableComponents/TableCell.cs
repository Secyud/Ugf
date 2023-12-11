using UnityEngine;

namespace Secyud.Ugf.TableComponents
{
    public class TableCell : MonoBehaviour
    {
        [SerializeField] private int Index;

        public int CellIndex
        {
            get => Index;
            set => Index = value;
        }

        public virtual void BindShowable(IShowable item)
        {
            
        }
    }
}
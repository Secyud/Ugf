using UnityEngine;

namespace Secyud.Ugf.TableComponents
{
    public abstract class TableComponentBase : MonoBehaviour
    {
        public abstract string Name { get; }
    }
    public abstract class TableComponentBase<TTableComponent,TTableComponentDelegate>:TableComponentBase
    where TTableComponentDelegate : TableComponentDelegateBase<TTableComponent,TTableComponentDelegate> 
    where TTableComponent : TableComponentBase<TTableComponent,TTableComponentDelegate>
    {
        public TTableComponentDelegate Delegate { get; internal set; }
    }
}
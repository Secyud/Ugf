using Secyud.Ugf.Abstraction;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.TableComponents.Components
{
    public interface ITableButtonDescriptor:
        IHasVisibility<object>
    {
        void SetButton(Button button,object target);
    }
}
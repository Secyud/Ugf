using UnityEngine.Events;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    public class SButton:Button
    {
        
        
        public void Bind(UnityAction action)
        {
            onClick.AddListener(action);
        }
    }
}
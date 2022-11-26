using Secyud.Ugf.Prefab;
using Secyud.Ugf.Prefab.Extension;
using UnityEngine.UI;

namespace Demo
{
    public class DemoPanel:PrefabBase
    {
        public Button DemoButton;
        
        [Dependency] private DemoDomainService DemoDomainService { get; set; }
        
        
        public override void OnInitialize()
        {
            DemoButton.onClick.AddListener(DomoMethod);
        }

        public void DomoMethod()
        {
            DemoDomainService.PrintMessage("domain service");
        }
    }
}
using Secyud.Ugf.Prefab;
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

        private void DomoMethod()
        {
            DemoDomainService.PrintMessage("domain service");
        }
    }
}
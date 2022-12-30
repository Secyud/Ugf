
using Secyud.Ugf.Unity.Prefabs;
using UnityEngine.UI;

namespace Demo
{
    public class DemoPanel:PrefabBase
    {
        public Button DemoButton;

        private DemoDomainService _demoDomainService;


        private void Awake()
        {
            DemoButton.onClick.AddListener(DomoMethod);
        }

        public override void OnInitialize()
        {
            _demoDomainService = Get<DemoDomainService>();
        }

        private void DomoMethod()
        {
            _demoDomainService.PrintMessage("domain service");
        }
    }
}
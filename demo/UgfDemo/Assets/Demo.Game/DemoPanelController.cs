

using Secyud.Ugf.Prefab;

namespace Demo
{
    public class DemoPanelController:PrefabControllerBase
    {
        private DemoPanel DemoPanel => GetComponent<DemoPanel>();

        private readonly DemoDomainService _demoDomainService;
        public DemoPanelController(DemoDomainService demoDomainService)
        {
            _demoDomainService = demoDomainService;
        }
        
        public override void OnInitialize()
        {
            var demoPanel = DemoPanel;
            demoPanel.DemoButton.onClick.AddListener(DomoMethod);
        }

        public void DomoMethod()
        {
            _demoDomainService.PrintMessage("domain service");
        }
    }
}
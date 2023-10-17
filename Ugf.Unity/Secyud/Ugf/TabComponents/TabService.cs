using System.Collections.Generic;
using System.Ugf.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.TabComponents
{
    [RegistryDisabled]
    public class TabService : IRegistry
    {
        public TabGroup TabGroup { get; set; }
        public List<TabPanel> Tabs { get; } = new();
        public void AddTab(TabPanel tabPanel)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].Name != tabPanel.Name)
                    continue;
                Tabs[i] = tabPanel;
                return;
            }
            
            Tabs.AddLast(tabPanel);
        }

        public void RefreshCurrentTab()
        {
            if (TabGroup)
            {
                TabGroup.RefreshCurrentTab();
            }
        }
    }
}
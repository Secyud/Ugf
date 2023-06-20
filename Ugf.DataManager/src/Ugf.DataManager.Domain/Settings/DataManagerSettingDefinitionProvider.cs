using Volo.Abp.Settings;

namespace Ugf.DataManager.Settings;

public class DataManagerSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(DataManagerSettings.MySetting1));
    }
}

using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Ugf.DataManager.Blazor;

[Dependency(ReplaceServices = true)]
public class DataManagerBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "DataManager";
}

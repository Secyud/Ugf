using Ugf.DataManager.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Ugf.DataManager.Blazor;

public abstract class DataManagerComponentBase : AbpComponentBase
{
    protected DataManagerComponentBase()
    {
        LocalizationResource = typeof(DataManagerResource);
    }
}

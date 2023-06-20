using Ugf.DataManager.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Ugf.DataManager.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DataManagerController : AbpControllerBase
{
    protected DataManagerController()
    {
        LocalizationResource = typeof(DataManagerResource);
    }
}

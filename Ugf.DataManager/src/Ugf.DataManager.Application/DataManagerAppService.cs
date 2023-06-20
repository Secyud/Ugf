using System;
using System.Collections.Generic;
using System.Text;
using Ugf.DataManager.Localization;
using Volo.Abp.Application.Services;

namespace Ugf.DataManager;

/* Inherit your application services from this class.
 */
public abstract class DataManagerAppService : ApplicationService
{
    protected DataManagerAppService()
    {
        LocalizationResource = typeof(DataManagerResource);
    }
}

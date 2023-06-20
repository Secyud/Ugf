using Ugf.DataManager.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Ugf.DataManager;

[DependsOn(
    typeof(DataManagerEntityFrameworkCoreTestModule)
    )]
public class DataManagerDomainTestModule : AbpModule
{

}

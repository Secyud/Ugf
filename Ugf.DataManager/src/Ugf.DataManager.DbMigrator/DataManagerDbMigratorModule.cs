using Ugf.DataManager.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Ugf.DataManager.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DataManagerEntityFrameworkCoreModule),
    typeof(DataManagerApplicationContractsModule)
    )]
public class DataManagerDbMigratorModule : AbpModule
{

}

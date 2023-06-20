using Volo.Abp.Modularity;

namespace Ugf.DataManager;

[DependsOn(
    typeof(DataManagerApplicationModule),
    typeof(DataManagerDomainTestModule)
    )]
public class DataManagerApplicationTestModule : AbpModule
{

}

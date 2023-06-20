using Ugf.DataManager.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Ugf.DataManager.Permissions;

public class DataManagerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DataManagerPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(DataManagerPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DataManagerResource>(name);
    }
}

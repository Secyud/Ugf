using System.Collections.Generic;
using System.Threading.Tasks;
using Ugf.DataManager.Localization;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

namespace Ugf.DataManager.Blazor.Menus;

public class DataManagerMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<DataManagerResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                DataManagerMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "fas fa-home",
                order: 0
            ));
        context.Menu.AddGroup(new ApplicationMenuGroup(
            DataManagerMenus.Class,
            l["Menu:Class"]));
        context.Menu.Items.InsertRange(1,new List<ApplicationMenuItem>
        {
            new(
                DataManagerMenus.ClassManagement,
                l["Menu:Class"],
                "/class",
                groupName:DataManagerMenus.Class,
                icon: "fas fa-home",
                order: 0
            ),
            new(
                DataManagerMenus.ObjectManagement,
                l["Menu:Object"],
                "/object",
                groupName:DataManagerMenus.Class,
                icon: "fas fa-home",
                order: 0
            ),
        });

        // if (MultiTenancyConsts.IsEnabled)
        // {
        //     administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        // }
        // else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

        return Task.CompletedTask;
    }
}

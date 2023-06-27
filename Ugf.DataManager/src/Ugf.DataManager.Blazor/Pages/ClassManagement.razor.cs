using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ugf.DataManager.ClassManagement;
using Ugf.DataManager.Localization;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Ugf.DataManager.Blazor.Pages;

public partial class ClassManagement
{
    public string SelectTab { get; set; } = "info";
    protected PageToolbar Toolbar { get; } = new();

    protected List<ClassPropertyDto> EditingProperties { get; set; } = new();
    protected List<TableColumn> ClassManagementTableColumns => TableColumns.Get<ClassManagement>();

    public ClassManagement()
    {
        LocalizationResource = typeof(DataManagerResource);
    }

    protected override ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Class"].Value));
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Classes"].Value));
        return base.SetBreadcrumbItemsAsync();
    }


    protected override ValueTask SetEntityActionsAsync()
    {
        EntityActions
            .Get<ClassManagement>()
            .AddRange(new EntityAction[]
            {
                new()
                {
                    Text = L["Edit"],
                    Clicked = async (data) => { await OpenEditModalAsync(data.As<ClassContainerDto>()); }
                },
                new()
                {
                    Text = L["Delete"],
                    Clicked = async data => await DeleteEntityAsync(data.As<ClassContainerDto>()),
                    ConfirmationMessage = data => GetDeleteConfirmationMessage(data.As<ClassContainerDto>())
                }
            });

        return base.SetEntityActionsAsync();
    }

    protected override ValueTask SetTableColumnsAsync()
    {
        ClassManagementTableColumns
            .AddRange(new TableColumn[]
            {
                new()
                {
                    Title = L["Actions"],
                    Actions = EntityActions.Get<ClassManagement>(),
                },
                new()
                {
                    Title = L["Name"],
                    Sortable = true,
                    Data = nameof(ClassContainerDto.Name)
                },
                new()
                {
                    Title = L["Description"],
                    Sortable = false,
                    Data = nameof(ClassContainerDto.Description)
                },
            });

        return base.SetTableColumnsAsync();
    }

    protected override string GetDeleteConfirmationMessage(ClassContainerDto entity)
    {
        return string.Format(L["ClassDeletionConfirmationMessage"], entity.Name);
    }

    protected override async Task OpenEditModalAsync(ClassContainerDto entity)
    {
        await base.OpenEditModalAsync(entity);
        EditingProperties = await AppService.GetPropertiesAsync(entity.Id);
    }

    private async Task RefreshClassProperty()
    {
        await AppService.CheckPropertiesAsync(EditingEntity.Id);
        EditingProperties = await AppService.GetPropertiesAsync(EditingEntity.Id);
        await InvokeAsync(StateHasChanged);
    }

    protected override ValueTask SetToolbarItemsAsync()
    {
        // Toolbar.AddButton(L["NewClass"],
        //     OpenCreateModalAsync,
        //     IconName.Add);

        return base.SetToolbarItemsAsync();
    }
}
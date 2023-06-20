using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using Ugf.DataManager.ClassManagement;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Ugf.DataManager.Blazor.Pages;

public partial class ObjectManagement
{
    protected PageToolbar Toolbar { get; } = new();

    protected List<TableColumn> ObjectManagementTableColumns => TableColumns.Get<ObjectManagement>();

    public string SearchClassName { get; set; }

    public ObjectManagement()
    {
        LocalizationResource = typeof(InfinityWorldChessDataResource);
    }

    protected override ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Class"].Value));
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Objects"].Value));
        return base.SetBreadcrumbItemsAsync();
    }

    protected override ValueTask SetEntityActionsAsync()
    {
        EntityActions
            .Get<ObjectManagement>()
            .AddRange(new EntityAction[]
            {
                new()
                {
                    Text = L["Edit"],
                    Clicked = async (data) => { await OpenEditModalAsync(data.As<SpecificObjectDto>()); }
                },
                new()
                {
                    Text = L["Data"],
                    Clicked = async (data) => { await OpenObjectDataModalAsync(data.As<SpecificObjectDto>()); }
                },
                new()
                {
                    Text = L["Delete"],
                    Clicked = async data => await DeleteEntityAsync(data.As<SpecificObjectDto>()),
                    ConfirmationMessage = data => GetDeleteConfirmationMessage(data.As<SpecificObjectDto>())
                }
            });

        return base.SetEntityActionsAsync();
    }

    protected override ValueTask SetTableColumnsAsync()
    {
        ObjectManagementTableColumns
            .AddRange(new TableColumn[]
            {
                new()
                {
                    Title = L["Actions"],
                    Actions = EntityActions.Get<ObjectManagement>(),
                },
                new()
                {
                    Title = L["Name"],
                    Sortable = true,
                    Data = nameof(SpecificObjectDto.Name)
                },
                new()
                {
                    Title = L["Bundle"],
                    Sortable = true,
                    Data = nameof(SpecificObjectDto.BundleName)
                },
            });

        return base.SetTableColumnsAsync();
    }

    protected override string GetDeleteConfirmationMessage(SpecificObjectDto entity)
    {
        return string.Format(L["ObjectDeletionConfirmationMessage"], entity.Name);
    }

    protected override ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["NewObject"],
            OpenCreateModalAsync,
            IconName.Add);

        return base.SetToolbarItemsAsync();
    }

    protected override async Task OpenCreateModalAsync()
    {
        await base.OpenCreateModalAsync();
        NewEntity.BundleName = Bundles.BundleNames.First();
    }

    public SpecificObjectDto DataDto { get; set; } = new();
    public ClassSelectComponent ClassSelect { get; set; }
    public Modal ObjectDataModal { get; set; }
    public Validations DataValidations { get; set; }

    private async Task CloseObjectDataModalAsync()
    {
        await InvokeAsync(ObjectDataModal.Hide);
    }

    private Task CloseObjectDataModal(ModalClosingEventArgs arg)
    {
        // cancel close if clicked outside of modal area
        arg.Cancel = arg.CloseReason == CloseReason.FocusLostClosing;

        return Task.CompletedTask;
    }

    private async Task OpenObjectDataModalAsync(SpecificObjectDto dto)
    {
        DataDto = await AppService.GetWithObjectDataAsync(dto.Id);
        await ObjectDataModal.Show();
    }

    private async Task UpdateObjectDataAsync()
    {
        if (await DataValidations.ValidateAll())
        {
            await AppService.UpdateObjectDataAsync(DataDto);
            await CloseObjectDataModalAsync();
        }
        else
        {
            await Message.Warn("Please check input!");
        }
    }

    private void ValidateValue(ValidatorEventArgs e, ObjectPropertyDto propertyDto)
    {
        if (propertyDto.Value.IsNullOrEmpty())
        {
            e.Status = ValidationStatus.Success;
        }
        else
        {
            string v = Convert.ToString(e.Value);

            e.Status = propertyDto.ClassProperty.Type switch
            {
                PropertyType.Bool => bool.TryParse(v, out _), PropertyType.UInt8 => byte.TryParse(v, out _),
                PropertyType.UInt16 => ushort.TryParse(v, out _), PropertyType.UInt32 => uint.TryParse(v, out _),
                PropertyType.UInt64 => ulong.TryParse(v, out _), PropertyType.Int8 => sbyte.TryParse(v, out _),
                PropertyType.Int16 => short.TryParse(v, out _), PropertyType.Int32 => int.TryParse(v, out _),
                PropertyType.Int64 => long.TryParse(v, out _), PropertyType.Single => float.TryParse(v, out _),
                PropertyType.Double => double.TryParse(v, out _), PropertyType.String => true,
                PropertyType.Guid => Guid.TryParse(v, out _), _ => false
            }
                ? ValidationStatus.Success
                : ValidationStatus.Error;
        }
    }

    private void SetClass(ClassContainerDto classContainerDto)
    {
        switch (_state)
        {
            case 0:
                GetListInput.ClassId = classContainerDto?.Id ?? default;
                SearchClassName = classContainerDto?.Namespace + '.' + classContainerDto?.Name;
                break;
            case 1:
                NewEntity.ClassId = classContainerDto?.Id ?? default;
                NewEntity.ClassContainer = classContainerDto;
                break;
            case 2:
                EditingEntity.ClassId = classContainerDto?.Id ?? default;
                EditingEntity.ClassContainer = classContainerDto;
                break;
        }

        StateHasChanged();
    }

    private int _state;

    private async void OpenClassSelect(int state)
    {
        _state = state;
        await ClassSelect.OpenModalAsync();
    }
}
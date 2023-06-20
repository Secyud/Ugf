using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Ugf.DataManager.ClassManagement;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Ugf.DataManager.Blazor.Pages;

public partial class ClassManagement
{
    public string SelectTab { get; set; } = "info";
    protected PageToolbar Toolbar { get; } = new();
    protected List<TableColumn> ClassManagementTableColumns => TableColumns.Get<ClassManagement>();
0
    private ClassSelectComponent ClassSelect { get; set; }
    public ConfigGeneratorComponent ConfigGenerator { get; set; }

    private List<StringContainerDto> NameSpaces { get; set; }


    public ClassManagement()
    {
        LocalizationResource = typeof(InfinityWorldChessDataResource);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        NameSpaces = await obj.ClassManagement.StringContainerAppService.GetNameSpacesAsync();
    }

    protected override ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Class"].Value));
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Classes"].Value));
        return base.SetBreadcrumbItemsAsync();
    }

    protected override ClassContainerCreateUpdateDto MapToEditingEntity(ClassContainerDto entityDto)
    {
        ClassContainerCreateUpdateDto dto = base.MapToEditingEntity(entityDto);

        try
        {
            if (entityDto.ParentId != default)
            {
                ClassContainerDto parent = AppService.GetAsync(entityDto.ParentId).Result;
                ParentName = parent.Name;
                dto.ParentChain = parent.Chains;
                dto.InheritProperties = AppService
                    .GetInheritListProperties(entityDto.ParentId)
                    .Result;
            }
            else
            {
                dto.InheritProperties = new List<ClassPropertyDto>();
            }
        }
        catch (Exception )
        {
            // ignored
        }

        return dto;
    }

    protected override async Task UpdateEntityAsync()
    {
        if (!EditingEntity.ParentContainsChain(EditingEntity.Id.ToString()))
            await base.UpdateEntityAsync();
        else
            await Message.Error("Parent is circle");
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
                    Title = L["Namespace"],
                    Sortable = true,
                    Data = nameof(ClassContainerDto.Namespace)
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


     [Inject] public ObjectManager Manager { get; set; }

    protected override ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["NewClass"],
            OpenCreateModalAsync,
            IconName.Add);
        Toolbar.AddButton(L["GenerateConfig"],
            () => ConfigGenerator.OpenModalAsync(),
            IconName.Desktop);
        Toolbar.AddButton(L["Update"],
            () => Manager.UpdateAllChainsAsync(),
            IconName.Desktop);

        return base.SetToolbarItemsAsync();
    }

    private async Task DeleteProperty(ClassPropertyDto context)
    {
        if (await Message.Confirm(L["DeleteClassPropertyConfirm"]))
            EditingEntity.Properties.Remove(context);
    }

    public ClassPropertyDto PropertyDto { get; set; } = new();
    public Modal CreateClassPropertyModal { get; set; }
    public Validations PropertyValidations { get; set; }

    private async Task AddPropertyAsync()
    {
        if (await PropertyValidations.ValidateAll())
        {
            EditingEntity.Properties.AddIfNotContains(PropertyDto);
            await ClosePropertyModalAsync();
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            await Message.Warn("Please check input!");
        }
    }

    private async Task ClosePropertyModalAsync()
    {
        await InvokeAsync(CreateClassPropertyModal.Hide);
    }

    private Task ClosePropertyModal(ModalClosingEventArgs arg)
    {
        // cancel close if clicked outside of modal area
        arg.Cancel = arg.CloseReason == CloseReason.FocusLostClosing;

        return Task.CompletedTask;
    }

    private async Task OpenPropertyModalAsync()
    {
        await CreateClassPropertyModal.Show();
    }

    private Task CreateProperty()
    {
        PropertyDto = new ClassPropertyDto
        {
            ClassId = EditingEntity.Id
        };
        return OpenPropertyModalAsync();
    }

    private Task EditProperty(ClassPropertyDto context)
    {
        PropertyDto = context;
        return OpenPropertyModalAsync();
    }

    private void ValidateIndex(ValidatorEventArgs e)
    {
        int v = Convert.ToInt32(e.Value);

        if (EditingEntity.Properties.Any(u => u.Index == v && u != PropertyDto) ||
            EditingEntity.InheritProperties.Any(u => u.Index == v))
            e.Status = ValidationStatus.Error;
        else
            e.Status = ValidationStatus.Success;
    }

    private string ParentName { get; set; }
    
    private async Task SetClassParent(ClassContainerDto classContainerDto)
    {
        switch (_state)
        {
            case 0:
                NewEntity.ParentChain = classContainerDto?.Chains;
                ParentName = classContainerDto?.Name;
                break;
            case 1:
                EditingEntity.ParentChain = classContainerDto?.Chains;
                ParentName = classContainerDto?.Name;
                EditingEntity.InheritProperties = classContainerDto is null
                    ? new List<ClassPropertyDto>()
                    : await AppService
                        .GetInheritListProperties(classContainerDto.Id);
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
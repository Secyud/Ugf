using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Ugf.DataManager.ClassManagement;
using Volo.Abp.Application.Dtos;

namespace Ugf.DataManager.Blazor.Pages;

public partial class ClassSelectComponent
{
    [Parameter] public EventCallback<ClassContainerDto> OnSelect { get; set; }
    public Modal Modal { get; set; }
    public ClassContainerDto Value { get; set; }
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; } = 12;
    public IReadOnlyList<ClassContainerDto> Entities { get; set; } = ArraySegment<ClassContainerDto>.Empty;
    public GetClassListInput GetListInput { get; } = new()
    {
        MaxResultCount = 12
    };
    private List<StringContainerDto> NameSpaces { get; set; } 

    protected override async Task OnInitializedAsync()
    {
        await GetEntitiesAsync();
        NameSpaces = await obj.ClassManagement.StringContainerAppService.GetNameSpacesAsync();
    }

    public async void ChangeValue(ClassContainerDto value)
    {
        Value = value;
        await InvokeAsync(StateHasChanged);
    }

    private async void OnDataGridReadAsync(DataGridReadDataEventArgs<ClassContainerDto> e)
    {
        GetListInput.Sorting =
            e.Columns.Where(c => c.SortDirection != 0)
                .Select(c => c.SortField + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
        GetListInput.MaxResultCount = e.PageSize == 0 ? PageSize : e.PageSize;
        GetListInput.SkipCount = e.Page * GetListInput.MaxResultCount;
        PagedResultDto<ClassContainerDto> listDto =
            await ClassAppService.GetListAsync(GetListInput);
        Entities = listDto.Items;
        TotalCount = (int)listDto.TotalCount;
        await InvokeAsync(StateHasChanged);
    }

    private async Task GetEntitiesAsync()
    {
        PagedResultDto<ClassContainerDto> listDto =
            await ClassAppService.GetListAsync(GetListInput);
        Entities = listDto.Items;
        TotalCount = (int)listDto.TotalCount;
    }
    
    private async Task CloseModalAsync()
    {
        await InvokeAsync(Modal.Hide);
    }

    private Task CloseModal(ModalClosingEventArgs arg)
    {
        // cancel close if clicked outside of modal area
        arg.Cancel = arg.CloseReason == CloseReason.FocusLostClosing;
        return Task.CompletedTask;
    }

    public async Task OpenModalAsync()
    {
        Value = null;

        await Modal.Show();
    }

    private async Task UpdateAsync()
    {
        if (Value is null)
            await OnSelect.InvokeAsync(null);
        else
        {
            ClassContainerDto v = await ClassAppService.GetAsync(Value.Id);
            await OnSelect.InvokeAsync(v);
        }
        await CloseModalAsync();
    }
}
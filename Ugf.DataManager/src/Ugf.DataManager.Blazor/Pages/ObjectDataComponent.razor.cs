using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Secyud.Ugf;
using Secyud.Ugf.DataManager;
using Ugf.DataManager.Blazor.ClassManagement;
using Ugf.DataManager.ClassManagement;

namespace Ugf.DataManager.Blazor.Pages;

public partial class ObjectDataComponent
{
    [Inject] public IClassContainerAppService ContainerAppService { get; set; }
    [Parameter] public object Object { get; set; }

    private ObjectDataView _objectDataView;

    protected override async Task OnInitializedAsync()
    {
        Guid id = TypeIdMapper.GetId(Object.GetType());
        ClassContainerDto classContainerDto = await ContainerAppService.GetAsync(id);
        _objectDataView = new ObjectDataView(Object, classContainerDto);
    }


    public async Task CreateAsync(Guid classId,Tuple<ClassPropertyDto, SAttribute> p)
    {
        _objectDataView.SetValue(p, U.Get(TypeIdMapper.GetType(classId)));
        await InvokeAsync(StateHasChanged);
    }
    public async Task SetNull(Tuple<ClassPropertyDto, SAttribute> p)
    {
        _objectDataView.SetValue(p, null);
        await InvokeAsync(StateHasChanged);
    }
}
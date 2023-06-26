using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Ugf.DataManager.ClassManagement;

namespace Ugf.DataManager.Blazor.Pages;

public partial class ConfigGeneratorComponent
{
    [Inject] public ISpecificObjectAppService ObjectAppService { get; set; }
    public Modal Modal { get; set; }
    private Guid ClassId { get; set; }
    private int? BundleId { get; set; }

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
        await Modal.Show();
    }

    private async Task GenerateConfigAsync()
    {
        if (ClassId == default)
        {
            await Message.Error("Please select class.");
            return;
        }

        await ObjectAppService.GenerateConfigAsync(ClassId, BundleId);
        await Notify.Success("Config Generate Successfully!");
        await CloseModalAsync();
    }
}
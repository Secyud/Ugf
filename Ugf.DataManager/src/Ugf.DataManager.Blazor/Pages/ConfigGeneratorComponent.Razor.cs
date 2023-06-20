using System;
using System.Threading.Tasks;
using Blazorise;
using Ugf.DataManager.ClassManagement;

namespace Ugf.DataManager.Blazor.Pages;

public partial class ConfigGeneratorComponent
{ 
    public Modal Modal { get; set; }
    public ClassContainerDto ClassContainer { get; set; }
    public string BundleName { get; set; } = Bundles.BundleNames.First();
    public ClassSelectComponent ClassSelect { get; set; }


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
        ClassContainer = null;

        await Modal.Show();
    }

    private async Task GenerateConfig()
    {
        if (ClassContainer is null)
        {
            await Message.Error("Please select class.");
            return;
        }

        if (BundleName.IsNullOrEmpty())
        {
            await Message.Error("Please select bundle.");
            return;
        }
        await ClassAppService.GenerateConfigAsync(ClassContainer.Id,BundleName);
        await Notify.Success("Config Generate Successfully!");
        await CloseModalAsync();
    }

    private void SetClass(ClassContainerDto containerDto)
    {
        ClassContainer = containerDto;
    }
}
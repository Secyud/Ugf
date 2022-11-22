namespace Secyud.Ugf.UserInterface
{
    public interface IUiControllerManager
    {
        TController Push<TController>() where TController: UiControllerBase;
        
        UiControllerBase Pop();
    }
}
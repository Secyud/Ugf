namespace Secyud.Ugf.UserInterface
{
    public interface IPrefabControllerManager
    {
        TController Add<TController>() where TController: PrefabControllerBase;
        
        PrefabControllerBase Remove<TController>()where TController: PrefabControllerBase;
    }
}
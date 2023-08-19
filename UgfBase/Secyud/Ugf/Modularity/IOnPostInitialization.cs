using System.Collections;

namespace Secyud.Ugf.Modularity
{
    public interface IOnPostInitialization
    {
        IEnumerator OnGamePostInitialization(GameInitializeContext context);
    }
}
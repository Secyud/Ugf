using System.Collections;

namespace Secyud.Ugf.Modularity
{
    public interface IOnPreInitialization
    {
        IEnumerator OnGamePreInitialization(GameInitializeContext context);
    }
}
using System.Collections;

namespace Secyud.Ugf.Modularity
{
    public interface IOnInitialization
    {
        IEnumerator OnGameInitializing(GameInitializeContext context);

        int GameInitializeStep { get; }
    }
}
using System.Collections;
using Secyud.Ugf.Modularity;

namespace Secyud.Ugf.Archiving
{
    public interface IOnInitialization
    {
        IEnumerator OnGameInitializing(GameInitializeContext context);
    }
}
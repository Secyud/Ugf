using System.Collections;
using Secyud.Ugf.Modularity;

namespace Secyud.Ugf.Archiving
{
    public interface IOnPreInitialization
    {
        IEnumerator OnGamePreInitialization(GameInitializeContext context);
    }
}
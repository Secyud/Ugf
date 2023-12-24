using System.Collections;
using Secyud.Ugf.Modularity;

namespace Secyud.Ugf.Archiving
{
    public interface IOnPostInitialization
    {
        IEnumerator OnGamePostInitialization(GameInitializeContext context);
    }
}
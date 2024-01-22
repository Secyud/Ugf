using System.Collections;

namespace Secyud.Ugf.Modularity
{
    public interface IGameModule
    {
        IEnumerable OnGameNewing();
        IEnumerable OnGameSaving();
        IEnumerable OnGameLoading();
    }
}
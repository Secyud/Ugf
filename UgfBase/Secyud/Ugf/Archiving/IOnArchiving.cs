using System.Collections;

namespace Secyud.Ugf.Archiving
{
    public interface IOnArchiving
    {
        IEnumerator SaveGame();
        IEnumerator LoadGame();
    }
}
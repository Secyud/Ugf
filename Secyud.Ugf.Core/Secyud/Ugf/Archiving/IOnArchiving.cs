using System.Threading.Tasks;

namespace Secyud.Ugf.Archiving
{
    public interface IOnArchiving
    {
        Task SaveGame();
        Task LoadGame();
    }
}
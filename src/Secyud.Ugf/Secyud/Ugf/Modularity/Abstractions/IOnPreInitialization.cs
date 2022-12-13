using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity
{
    public interface IOnPreInitialization
    {
        Task OnGamePreInitializationAsync(InitializationContext context);
    }
}
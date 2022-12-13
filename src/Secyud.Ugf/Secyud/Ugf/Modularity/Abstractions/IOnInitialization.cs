using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity
{
    public interface IOnInitialization
    {
        Task OnGameInitializationAsync(InitializationContext context);
    }
}
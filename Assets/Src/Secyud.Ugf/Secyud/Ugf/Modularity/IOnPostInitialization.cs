using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity
{
    public interface IOnPostInitialization
    {
        Task OnGamePostInitializationAsync(InitializationContext context);
    }
}
using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity
{
    public interface IOnShutdown
    {
        Task OnShutdownAsync(ShutdownContext context);
    }
}
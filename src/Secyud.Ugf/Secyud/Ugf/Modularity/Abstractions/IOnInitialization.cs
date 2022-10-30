using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity;

public interface IOnInitialization
{
    Task OnInitializationAsync(InitializationContext context);
}
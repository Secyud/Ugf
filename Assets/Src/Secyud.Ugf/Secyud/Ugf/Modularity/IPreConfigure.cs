using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity
{
    public interface IPreConfigure
    {
        Task PreConfigureGameAsync(ConfigurationContext context);
    }
}
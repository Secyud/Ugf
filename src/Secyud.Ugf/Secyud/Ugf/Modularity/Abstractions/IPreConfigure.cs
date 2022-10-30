using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity;

public interface IPreConfigure
{
    Task PreConfigureAsync(ConfigurationContext context);
}
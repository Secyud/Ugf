using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity
{
    public interface IPostConfigure
    {
        Task PostConfigureAsync(ConfigurationContext context);
    }
}
using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity
{
    public interface IUgfModule
    {
        Task ConfigureAsync(ConfigurationContext context);

        void Configure(ConfigurationContext context);
    }
}
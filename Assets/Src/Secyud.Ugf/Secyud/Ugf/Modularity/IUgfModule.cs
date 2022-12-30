using System.Threading.Tasks;

namespace Secyud.Ugf.Modularity
{
    public interface IUgfModule
    {
        Task ConfigureGameAsync(ConfigurationContext context);

        void ConfigureGame(ConfigurationContext context);
    }
}
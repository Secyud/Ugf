using System.Threading.Tasks;

namespace Ugf.DataManager.Data;

public interface IDataManagerDbSchemaMigrator
{
    Task MigrateAsync();
}

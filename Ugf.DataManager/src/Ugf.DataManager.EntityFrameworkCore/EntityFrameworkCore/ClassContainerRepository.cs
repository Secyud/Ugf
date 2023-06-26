using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Secyud.Ugf.DataManager;
using Ugf.DataManager.ClassManagement;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Ugf.DataManager.EntityFrameworkCore;

public class ClassContainerRepository :
    EfCoreRepository<DataManagerDbContext, ClassContainer, Guid>,
    IClassContainerRepository
{
    public ClassContainerRepository(
        IDbContextProvider<DataManagerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<ClassContainer>> GetListAsync(
        int skipCount, int maxResultCount, string sorting,
        string name, Guid classId,
        bool includeDetails = false, CancellationToken token = default)
    {
        IQueryable<ClassContainer> query =
            includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();

        IQueryable<ClassContainer> results =
            (await FilteredQueryableAsync(query, name, classId))
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount);

        List<ClassContainer> list = results.ToList();

        return list;
    }

    public Task<IQueryable<ClassContainer>> FilteredQueryableAsync(
        IQueryable<ClassContainer> queryable, string name, Guid classId)
    {
        IQueryable<ClassContainer> results =
            queryable.WhereIf(!name.IsNullOrEmpty(),
                u => u.Name.Contains(name));

        if (classId != default)
        {
            Type type = TypeIdMapper.GetType(classId);

            results = results.Where(u =>
                TypeIdMapper.GetType(u.Id).IsAssignableTo(type));
        }

        return Task.FromResult(results);
    }
}
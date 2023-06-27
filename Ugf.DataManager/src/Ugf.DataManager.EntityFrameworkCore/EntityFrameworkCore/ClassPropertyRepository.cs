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

public class ClassPropertyRepository :
    EfCoreRepository<DataManagerDbContext, ClassProperty, Guid>,
    IClassPropertyRepository
{
    public ClassPropertyRepository(
        IDbContextProvider<DataManagerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<ClassProperty>> GetListAsync(
        int skipCount, int maxResultCount, string sorting,
        Guid classId,
        bool includeDetails = false, CancellationToken token = default)
    {
        IQueryable<ClassProperty> query =
            includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();

        IQueryable<ClassProperty> results =
            (await FilteredQueryableAsync(query, classId))
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount);

        List<ClassProperty> list = results.ToList();

        return list;
    }

    public Task<IQueryable<ClassProperty>> FilteredQueryableAsync(
        IQueryable<ClassProperty> queryable, Guid classId)
    {
        IQueryable<ClassProperty> results = queryable;

        if (classId != default)
        {
            Type type = TypeIdMapper.GetType(classId);

            results = results.Where(u =>
                TypeIdMapper.GetType(u.Id).IsAssignableTo(type));
        }

        return Task.FromResult(results);
    }
}
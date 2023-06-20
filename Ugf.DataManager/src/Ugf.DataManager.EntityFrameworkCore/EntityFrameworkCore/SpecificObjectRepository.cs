using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Secyud.Ugf.DataManager;
using Ugf.DataManager.ClassManagement;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Ugf.DataManager.EntityFrameworkCore;

public class SpecificObjectRepository :
    EfCoreRepository<DataManagerDbContext, SpecificObject, Guid>,
    ISpecificObjectRepository
{
    public SpecificObjectRepository(
        IDbContextProvider<DataManagerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<SpecificObject>> GetListAsync(
        int skipCount, int maxResultCount, string sorting,
        string name, int? bundleId, Guid classId,
        bool includeDetails = false, CancellationToken token = default)
    {
        IQueryable<SpecificObject> query =
            includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();

        IQueryable<SpecificObject> results =
            (await FilteredQueryableAsync(query, name, bundleId, classId))
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount);

        return await results.ToListAsync(cancellationToken: token);
    }

    public async Task<IQueryable<SpecificObject>> FilteredQueryableAsync(
        IQueryable<SpecificObject> query,
        string name, int? bundleId, Guid classId)
    {
        IQueryable<SpecificObject> results = (await GetQueryableAsync())
            .WhereIf(!name.IsNullOrEmpty(), u => u.Name.Contains(name))
            .WhereIf(bundleId is not null, u => u.BundleId == bundleId);

        if (classId != default)
        {
            Type type = TypeIdMapper.GetType(classId);

            results = results.Where(
                u => TypeIdMapper.GetType(u.ClassId).IsAssignableTo(type));
        }

        return results;
    }
}
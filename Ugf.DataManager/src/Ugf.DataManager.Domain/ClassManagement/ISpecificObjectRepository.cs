using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Ugf.DataManager.ClassManagement;

public interface ISpecificObjectRepository : IRepository<SpecificObject, Guid>
{
    Task<List<SpecificObject>> GetListAsync(
        int skipCount, int maxResultCount, string sorting,
        string name, int? bundleId, Guid classId,
        bool includeDetails = false, CancellationToken token = default);

    Task<IQueryable<SpecificObject>> FilteredQueryableAsync(
        IQueryable<SpecificObject> query,
        string name, int? bundleId, Guid classId);
}
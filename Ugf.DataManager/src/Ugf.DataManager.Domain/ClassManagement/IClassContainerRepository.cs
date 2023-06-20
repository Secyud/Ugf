using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Ugf.DataManager.ClassManagement;

public interface IClassContainerRepository : IRepository<ClassContainer, Guid>
{
    Task<List<ClassContainer>> GetListAsync(
        int skipCount, int maxResultCount, string sorting,
        string name, Guid classId,
        bool includeDetails = false, CancellationToken token = default);

    Task<IQueryable<ClassContainer>> FilteredQueryableAsync(
        IQueryable<ClassContainer> queryable, string name, Guid classId);
}
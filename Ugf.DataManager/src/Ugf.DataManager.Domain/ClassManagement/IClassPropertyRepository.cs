using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Ugf.DataManager.ClassManagement;

public interface IClassPropertyRepository : IRepository<ClassProperty, Guid>
{
    Task<List<ClassProperty>> GetListAsync(
        int skipCount, int maxResultCount, string sorting,
        Guid classId,
        bool includeDetails = false, CancellationToken token = default);

    Task<IQueryable<ClassProperty>> FilteredQueryableAsync(
        IQueryable<ClassProperty> queryable, Guid classId);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ugf.DataManager.ClassManagement;

public class SpecificObjectAppService :
    CrudAppService<SpecificObject, SpecificObjectDto, Guid, GetObjectListInput>,
    ISpecificObjectAppService
{
    private readonly ISpecificObjectRepository _repository;
    private readonly ObjectManager _objectManager;

    public SpecificObjectAppService(ISpecificObjectRepository repository, ObjectManager objectManager)
        : base(repository)
    {
        _repository = repository;
        _objectManager = objectManager;
    }

    protected override async Task<IQueryable<SpecificObject>> CreateFilteredQueryAsync(GetObjectListInput input)
    {
        return await _repository.FilteredQueryableAsync(
            await _repository.GetQueryableAsync(),
            input.Name, input.BundleId, input.ClassId);
    }

    public Task GenerateConfigAsync(Guid id, int? bundleId)
    {
        return _objectManager.GenerateConfigAsync(id, bundleId);
    }
}
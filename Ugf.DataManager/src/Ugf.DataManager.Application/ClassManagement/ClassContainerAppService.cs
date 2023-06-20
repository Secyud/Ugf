using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Ugf.DataManager.ClassManagement;

public class ClassContainerAppService :
    CrudAppService<ClassContainer, ClassContainerDto, Guid,
        GetClassListInput>, IClassContainerAppService
{
    private readonly IClassContainerRepository _repository;

    public ClassContainerAppService(
        IClassContainerRepository repository) : base(repository)
    {
        _repository = repository;
    }



    protected override async Task<IQueryable<ClassContainer>> CreateFilteredQueryAsync(GetClassListInput input)
    {
        return await _repository.FilteredQueryableAsync(
            await _repository.GetQueryableAsync(),
            input.Name, input.ClassId);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Ugf.DataManager.ClassManagement;

public interface IClassContainerAppService:
    ICrudAppService<ClassContainerDto,Guid,GetClassListInput>
{
    Task CheckPropertiesAsync(Guid id);
    Task<List<ClassPropertyDto>> GetPropertiesAsync(Guid id);
    Task UpdatePropertiesAsync(List<ClassPropertyDto> properties);
}
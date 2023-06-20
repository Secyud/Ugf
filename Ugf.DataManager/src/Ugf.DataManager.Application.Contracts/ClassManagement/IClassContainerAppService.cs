using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Ugf.DataManager.ClassManagement;

public interface IClassContainerAppService:
    ICrudAppService<ClassContainerDto,Guid,GetClassListInput>
{
}
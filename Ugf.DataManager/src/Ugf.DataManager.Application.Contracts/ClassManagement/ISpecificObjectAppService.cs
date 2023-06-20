using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Ugf.DataManager.ClassManagement;

public interface ISpecificObjectAppService :
    ICrudAppService<SpecificObjectDto, Guid, GetObjectListInput>
{
    Task GenerateConfigAsync(Guid id, int? bundleId);
}
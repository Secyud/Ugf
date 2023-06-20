using Volo.Abp.Application.Dtos;

namespace Ugf.DataManager.ClassManagement;

public class GetStringListInput:PagedAndSortedResultRequestDto
{
    public string Name { get; set; }
}
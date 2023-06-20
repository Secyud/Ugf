using System;
using Volo.Abp.Application.Dtos;

namespace Ugf.DataManager.ClassManagement;

public class GetClassListInput:PagedAndSortedResultRequestDto
{
    public string Name { get; set; }
    public Guid ClassId { get; set; }
}
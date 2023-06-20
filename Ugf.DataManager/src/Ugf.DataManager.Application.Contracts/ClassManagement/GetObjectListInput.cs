using System;
using Volo.Abp.Application.Dtos;

namespace Ugf.DataManager.ClassManagement;

public class GetObjectListInput:PagedAndSortedResultRequestDto
{
    public string Name { get; set; }
    public int? BundleId { get; set; }
    public Guid ClassId { get; set; }
}
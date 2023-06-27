using System;
using Volo.Abp.Application.Dtos;

namespace Ugf.DataManager.ClassManagement;

public class ClassPropertyDto : EntityDto<Guid>
{
    public Guid ClassId { get; set; }
    public short PropertyId { get; set; }
    
    public byte DataType { get; set; }
    public string PropertyName { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
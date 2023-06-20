using System;
using Volo.Abp.Application.Dtos;

namespace Ugf.DataManager.ClassManagement;

public class ClassPropertyDto : EntityDto
{
    public Guid ClassId { get; set; }
    public short PropertyId { get; set; }
    public string PropertyName { get; set; }
    public string Description { get; set; }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Ugf.DataManager.ClassManagement;

public class ClassContainerDto:EntityDto<Guid>
{
    [Required]
    public string Name { get;  set; }
    public string Description { get; set; }
    public List<ClassPropertyDto> Properties { get; set; }
}
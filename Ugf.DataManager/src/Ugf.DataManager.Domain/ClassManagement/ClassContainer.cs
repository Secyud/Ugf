using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace Ugf.DataManager.ClassManagement;

public class ClassContainer : FullAuditedAggregateRoot<Guid>
{
    private ClassContainer()
    {
    }

    public ClassContainer(
        Guid id, string name, string description)
        : base(id)
    {
        Name = name;
        Description = description;
        Properties = new List<ClassProperty>();
    }

    public Guid ClassId { get; set; }
    public string Name { get;  set; }
    public string Description { get; set; }
    public List<ClassProperty> Properties { get; set; }
}
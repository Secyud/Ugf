using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Ugf.DataManager.ClassManagement;

public class ClassContainer : FullAuditedAggregateRoot<Guid>
{
    private ClassContainer()
    {
    }

    public ClassContainer(
        Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    public string Name { get; set; }
    public string Description { get; set; }
}
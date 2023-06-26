using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Ugf.DataManager.ClassManagement;

public class SpecificObject:FullAuditedAggregateRoot<Guid>
{
    private SpecificObject(byte[] archivedData, byte[] initialedData, byte[] ignoredData)
    {
        ArchivedData = archivedData;
        InitialedData = initialedData;
        IgnoredData = ignoredData;
    }
    
    public SpecificObject(Guid id, 
        Guid classId, string name, int bundleId, byte[] archivedData, byte[] initialedData, byte[] ignoredData) 
        : base(id)
    {
        ClassId = classId;
        Name = name;
        BundleId = bundleId;
        ArchivedData = archivedData;
        InitialedData = initialedData;
        IgnoredData = ignoredData;
    }

    public Guid ClassId { get; set; }
    public string Name { get; set; }
    public int BundleId { get; set; }
    public byte[] ArchivedData { get; set; }
    public byte[] InitialedData { get; set; }
    public byte[] IgnoredData { get; set; }
}
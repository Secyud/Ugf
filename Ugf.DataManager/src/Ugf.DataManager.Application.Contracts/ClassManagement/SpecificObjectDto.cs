using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Ugf.DataManager.ClassManagement;

public class SpecificObjectDto:EntityDto<Guid>
{
    public Guid ClassId { get; set; }
    public string Name { get; set; }
    public int BundleId { get; set; }
    public byte[] ArchivedData { get; set; }
    public byte[] InitialedData { get; set; }
    public byte[] IgnoredData { get; set; }
}
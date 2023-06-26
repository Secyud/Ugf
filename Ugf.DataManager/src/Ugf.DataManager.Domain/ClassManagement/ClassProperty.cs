using System;
using Secyud.Ugf.DataManager;
using Volo.Abp.Domain.Entities;

namespace Ugf.DataManager.ClassManagement;

public class ClassProperty : Entity
{
    private ClassProperty()
    {
    }

    public ClassProperty(Guid classId, short propertyId,DataType dataType, string propertyName)
    {
        ClassId = classId;
        PropertyId = propertyId;
        DataType = dataType;
        PropertyName = propertyName;
    }

    public Guid ClassId { get; set; }
    public short PropertyId { get; set; }
    public DataType DataType { get; set; }
    public string PropertyName { get; set; }
    public string Description { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { ClassId, PropertyId,DataType };
    }
}
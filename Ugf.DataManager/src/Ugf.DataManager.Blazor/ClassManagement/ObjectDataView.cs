using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf;
using Secyud.Ugf.DataManager;
using Ugf.DataManager.ClassManagement;

namespace Ugf.DataManager.Blazor.ClassManagement;

public class ObjectDataView
{
    public List<Tuple<ClassPropertyDto, SAttribute>> Properties { get; } = new();
    public PropertyDescriptor Descriptor { get; }
    public object Obj { get; }

    public ObjectDataView(object obj, ClassContainerDto containerDto)
    {
        Type type = TypeIdMapper.GetType(containerDto.Id);
        Descriptor = U.Factory.InitializeManager.GetProperty(type);

        AddProperty(Descriptor.ArchiveProperties, containerDto);
        AddProperty(Descriptor.InitialedProperties, containerDto);
        AddProperty(Descriptor.IgnoredProperties, containerDto);

        Obj = obj;
    }

    private void AddProperty(SAttribute[] attributes, ClassContainerDto containerDto)
    {
        foreach (SAttribute attribute in attributes)
        {
            ClassPropertyDto p = containerDto.Properties.FirstOrDefault(u =>
                u.PropertyId == attribute.ID && u.DataType == (int)attribute.DataType);
            Properties.Add(new Tuple<ClassPropertyDto, SAttribute>(p, attribute));
        }
    }
    
    public TValue GetValue<TValue>(Tuple<ClassPropertyDto, SAttribute> p)
    {
        return (TValue)p.Item2.GetValue(Obj);
    }

    public void SetValue(Tuple<ClassPropertyDto, SAttribute> p, object value)
    {
        p.Item2.SetValue(Obj, value);
    }
}
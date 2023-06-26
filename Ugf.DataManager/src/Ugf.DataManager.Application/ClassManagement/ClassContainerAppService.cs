using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Secyud.Ugf;
using Secyud.Ugf.DataManager;
using Volo.Abp.Application.Services;

namespace Ugf.DataManager.ClassManagement;

public class ClassContainerAppService :
    CrudAppService<ClassContainer, ClassContainerDto, Guid,
        GetClassListInput>, IClassContainerAppService
{
    private readonly IClassContainerRepository _repository;

    public ClassContainerAppService(
        IClassContainerRepository repository) : base(repository)
    {
        _repository = repository;
    }

    protected override async Task<IQueryable<ClassContainer>> CreateFilteredQueryAsync(GetClassListInput input)
    {
        return await _repository.FilteredQueryableAsync(
            await _repository.GetQueryableAsync(),
            input.Name, input.ClassId);
    }

    protected override async Task<ClassContainer> GetEntityByIdAsync(Guid id)
    {
        ClassContainer entity = await _repository.FindAsync(id) ?? await CreateNewAsync(id);

        return entity;
    }

    public async Task<ClassContainerDto> CheckPropertiesAsync(Guid id)
    {
        ClassContainer c = await _repository.FindAsync(id);

        if (c is null)
            c = await CreateNewAsync(id);
        else
        {
            Type type = TypeIdMapper.GetType(id);
            PropertyDescriptor properties = U.Factory.InitializeManager.GetProperty(type);
            await CheckPropertiesAsync(c,properties);
        }
        
        return await MapToGetOutputDtoAsync(c);
    }

    private async Task<ClassContainer> CreateNewAsync(Guid id)
    {
        Type type = TypeIdMapper.GetType(id);
        
        PropertyDescriptor properties = U.Factory.InitializeManager.GetProperty(type);
        
        ClassContainer c = await _repository.InsertAsync(new ClassContainer(id, type.Name), true);

        await CheckPropertiesAsync(c,properties);
        
        return c;
    }

    private async Task CheckPropertiesAsync(ClassContainer container,PropertyDescriptor properties)
    {
        List<ClassProperty> addedProperties = new();

        void CheckAttribute(SAttribute[] attributes)
        {
            foreach (SAttribute attribute in attributes)
            {
                ClassProperty cp = container.Properties.Find(u =>
                    u.PropertyId == attribute.ID &&
                    u.DataType == attribute.DataType);

                if (cp is null)
                    addedProperties.Add(new ClassProperty(
                        container.Id, attribute.ID, attribute.DataType, attribute.Info.Name));
                else
                {
                    cp.PropertyName = attribute.Info.Name;
                    addedProperties.Add(cp);
                }
            }
        }

        CheckAttribute(properties.ArchiveProperties);
        CheckAttribute(properties.InitialedProperties);
        CheckAttribute(properties.IgnoredProperties);
        container.Properties.Clear();
        container.Properties.AddRange(addedProperties);

        await _repository.UpdateAsync(container);
    }
}
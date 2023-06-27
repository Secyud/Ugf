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
    private readonly IClassPropertyRepository _propertyRepository;
    private readonly IClassContainerRepository _repository;

    public ClassContainerAppService(
        IClassPropertyRepository propertyRepository,
        IClassContainerRepository repository) : base(repository)
    {
        _propertyRepository = propertyRepository;
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
        ClassContainer entity = 
            await _repository.FindAsync(id) ?? 
            await CreateNewAsync(id);
        return entity;
    }

    public async Task CheckPropertiesAsync(Guid id)
    {
        List<ClassProperty> deleteProperties = await GetPropertiesByIdAsync(id);
        List<ClassProperty> insertProperties = new();
        List<ClassProperty> updateProperties = new();
        
        Type type = TypeIdMapper.GetType(id);
        PropertyDescriptor descriptor = U.Factory.InitializeManager.GetProperty(type);

        void CheckAttribute(SAttribute[] attributes)
        {
            foreach (SAttribute attribute in attributes)
            {
                ClassProperty cp = deleteProperties.Find(u =>
                    u.PropertyId == attribute.ID &&
                    u.DataType == attribute.DataType);

                if (cp is null)
                    insertProperties.Add(new ClassProperty(
                        GuidGenerator.Create(),TypeIdMapper.GetId(attribute.Belong), 
                        attribute.ID, attribute.DataType, attribute.Info.Name));
                else
                {
                    deleteProperties.Remove(cp);
                    updateProperties.Add(cp);
                    cp.ClassId = TypeIdMapper.GetId(attribute.Belong);
                }
            }
        }

        CheckAttribute(descriptor.ArchiveProperties);
        CheckAttribute(descriptor.InitialedProperties);
        CheckAttribute(descriptor.IgnoredProperties);

        await _propertyRepository.DeleteManyAsync(deleteProperties, true);
        await _propertyRepository.UpdateManyAsync(updateProperties, true);
        await _propertyRepository.InsertManyAsync(insertProperties, true);
    }

    public async Task<List<ClassPropertyDto>> GetPropertiesAsync(Guid id)
    {
        return ObjectMapper.Map<List<ClassProperty>, List<ClassPropertyDto>>(
            await GetPropertiesByIdAsync(id));
    }

    public async Task UpdateProperties(List<ClassPropertyDto> properties)
    {
        List<ClassProperty> entities = ObjectMapper.Map<List<ClassPropertyDto>, List<ClassProperty>>(
            properties);

        await _propertyRepository.UpdateManyAsync(entities);
    }

    private async Task<List<ClassProperty>> GetPropertiesByIdAsync(Guid id)
    {
        List<Guid> findIds = new();
        Type type = TypeIdMapper.GetType(id);

        while (type is not null && type != typeof(object))
        {
            findIds.Add(TypeIdMapper.GetId(type));
            type = type.BaseType;
        }

        return 
            (await _propertyRepository.GetQueryableAsync())
            .Where(u => findIds.Contains(u.ClassId))
            .ToList();
    }

    private async Task<ClassContainer> CreateNewAsync(Guid classId)
    {
        Type type = TypeIdMapper.GetType(classId);

        ClassContainer c = await _repository.InsertAsync(
            new ClassContainer(classId,type.Name));

        await CheckPropertiesAsync(classId);
        
        return c;
    }
}
using AutoMapper;
using Ugf.DataManager.ClassManagement;

namespace Ugf.DataManager;

public class DataManagerApplicationAutoMapperProfile : Profile
{
    public DataManagerApplicationAutoMapperProfile()
    {
        CreateMap<ClassProperty, ClassPropertyDto>();
        CreateMap<ClassContainer, ClassContainerDto>();
        CreateMap<SpecificObject, SpecificObjectDto>();
    }
}

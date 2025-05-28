using AutoMapper;
using BLL.EntityBLLModels;
using DTOsLibrary.DTOEnums;
using DTOsLibrary;

namespace WebPresentation.AutoMapperProfiles;

public class BaseUserProfile : Profile
{
    public BaseUserProfile()
    {
        CreateMap<EnumInterfaceTypeDto, BusinessEnumInterfaceType>().ReverseMap();

        CreateMap<BaseUserModel, BaseUserDto>().ReverseMap();
    }
}

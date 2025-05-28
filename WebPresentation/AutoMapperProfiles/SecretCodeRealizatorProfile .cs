using AutoMapper;
using BLL.EntityBLLModels;
using DTOsLibrary;
using DTOsLibrary.DTOEnums;

namespace WebPresentation.AutoMapperProfiles;

public class SecretCodeRealizatorProfile : Profile
{
    public SecretCodeRealizatorProfile()
    {
        CreateMap<EnumInterfaceTypeDto, BusinessEnumInterfaceType>().ReverseMap();

        CreateMap<SecretCodeRealizatorModel, SecretCodeRealizatorDto>()
            .ForMember(dest => dest.RealizatorType, opt => opt.MapFrom(src => src.RealizatorType))
            .ReverseMap();
    }
}

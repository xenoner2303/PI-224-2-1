using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles;

public class SecretCodeRealizatorProfile : Profile
{
    public SecretCodeRealizatorProfile()
    {
        CreateMap<EnumUserInterfaceType, BusinessEnumInterfaceType>().ReverseMap();

        CreateMap<AbstractSecretCodeRealizator, SecretCodeRealizatorModel>()
           .ForMember(dest => dest.SecretCode, opt => opt.Ignore()) // ігноруємо поле SecretCode
           .ForMember(dest => dest.RealizatorType, opt => opt.MapFrom(src => src.InterfaceType));
    }
}

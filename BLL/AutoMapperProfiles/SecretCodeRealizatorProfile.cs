using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles;

public class SecretCodeRealizatorProfile : Profile
{
    public SecretCodeRealizatorProfile()
    {
        CreateMap<AbstractSecretCodeRealizator, SecretCodeRealizatorModel>()
           .ForMember(dest => dest.SecretCode, opt => opt.Ignore()) // ігноруємо поле SecretCode
           .ForMember(dest => dest.RealizatorType, opt => opt.MapFrom(src => MapRealizatorType(src)));
    }

    private BusinessEnumInterfaceType MapRealizatorType(AbstractSecretCodeRealizator realizator)
    {
        return realizator switch
        {
            ManagerSecretCodeRealizator => BusinessEnumInterfaceType.Manager,
            AdministratorSecretCodeRealization => BusinessEnumInterfaceType.Administrator,
            _ => throw new ArgumentException("Некоректний тип реалізатору")
        };
    }
}

using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles;

internal class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<EnumUserInterfaceType, BusinessEnumInterfaceType>().ReverseMap();

        // зворотній з AbstrUser у BaseUserModel
        CreateMap<AbstrUser, BaseUserModel>()
            .ForMember(dest => dest.Password, opt => opt.Ignore()) // пароль не повертаємо
            .ForMember(dest => dest.InterfaceType, opt => opt.MapFrom(src => src.InterfaceType));
    }
}
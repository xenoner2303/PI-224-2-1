using AutoMapper;
using BLL.AutoMapperProfiles.ValueResolvers;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles;

public class BidProfile : Profile
{
    public BidProfile()
    {
        CreateMap<Bid, BidModel>();

        CreateMap<BidModel, Bid>()
            .ForMember(dest => dest.User, opt => opt.Ignore()) // ігноруємо мапінг Owner
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id)); // явно задаємо OwnerId
    }
}
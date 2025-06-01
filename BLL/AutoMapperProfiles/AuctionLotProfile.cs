using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Entities;
using BLL.AutoMapperProfiles.ValueResolvers;

namespace BLL.AutoMapperProfiles;

public class AuctionLotProfile : Profile
{
    public AuctionLotProfile()
    {
        CreateMap<EnumLotStatuses, BusinessEnumLotStatuses>().ReverseMap();

        CreateMap<AuctionLot, AuctionLotModel>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom<ImageToBytesImageModelResolver>());

        CreateMap<AuctionLotModel, AuctionLot>()
            .ForMember(dest => dest.RelativeImagePath, opt => opt.MapFrom<BytesToImageResolver>())
            .ForMember(dest => dest.Owner, opt => opt.Ignore()) // ігноруємо мапінг Owner
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Owner.Id)); // явно задаємо OwnerId
    }
}

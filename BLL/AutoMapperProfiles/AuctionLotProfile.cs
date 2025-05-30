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
            .ForMember(dest => dest.RelativeImagePath, opt => opt.MapFrom<BytesToImageResolver>());
    }
}

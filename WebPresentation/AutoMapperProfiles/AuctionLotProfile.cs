using AutoMapper;
using BLL.EntityBLLModels;
using DTOsLibrary;
using DTOsLibrary.DTOEnums;

namespace WebPresentation.AutoMapperProfiles;

public class AuctionLotProfile : Profile
{
    public AuctionLotProfile()
    {
        CreateMap<EnumLotStatusesDto, BusinessEnumLotStatuses>().ReverseMap();

        CreateMap<AuctionLotModel, AuctionLotDto>().ReverseMap();
    }
}

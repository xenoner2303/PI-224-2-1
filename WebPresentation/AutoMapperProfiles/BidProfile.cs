using AutoMapper;
using BLL.EntityBLLModels;
using DTOsLibrary;

namespace WebPresentation.AutoMapperProfiles;

public class BidProfile : Profile
{
    public BidProfile()
    {
        CreateMap<BidModel, BidDto>().ReverseMap();
    }
}

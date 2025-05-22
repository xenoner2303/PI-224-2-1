using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles;

public class BidProfile : Profile
{
    public BidProfile()
    {
        CreateMap<Bid, BidModel>()
            .ReverseMap();
    }
}
using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryModel>()
            .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories))
            .ForMember(dest => dest.Lots, opt => opt.MapFrom(src => src.Lots))
            .ReverseMap();
    }
}

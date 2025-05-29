using AutoMapper;
using BLL.EntityBLLModels;
using DTOsLibrary;

namespace WebPresentation.AutoMapperProfiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryModel, CategoryDto>().ReverseMap();
    }
}

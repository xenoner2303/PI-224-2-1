using AutoMapper;
using BLL.AutoMapperProfiles.ValueResolvers;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles;
public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryModel>().ReverseMap();

       /* CreateMap<IEnumerable<Category>, List<CategoryModel>>()
            .ConvertUsing<CategoriesListResolver>();
       */
    }
}

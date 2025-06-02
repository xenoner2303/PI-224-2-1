using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles.ValueResolvers;

internal class CategoriesListResolver : ITypeConverter<IEnumerable<Category>, List<CategoryModel>>
{
    public List<CategoryModel> Convert(IEnumerable<Category> source, List<CategoryModel> destination, ResolutionContext context)
    {
        List<CategoryModel> result = new List<CategoryModel>();

        foreach (var srcCat in source)
        {
            // мапимо одиничну категорію стандартним шляхом
            var mapped = context.Mapper.Map<CategoryModel>(srcCat);

            if (!MergeIntoSubcategories(mapped, result))
            {
                // якщо підкатегорії не були злиті, то додаємо до результату
                result.Add(mapped);
            }
        }

        return result;
    }

    private bool MergeIntoSubcategories(CategoryModel target, List<CategoryModel> newSubs)
    {
        if (newSubs == null || newSubs.Count == 0)
        {
            return false; // нічого не потрібно зливати
        }

        foreach (var sub in newSubs)
        {
            var existing = sub.Subcategories.FindIndex(c => c.Id == target.Id);

            if (existing != -1)
            {
                sub.Subcategories[existing].Subcategories = target.Subcategories; // поновлюємо підкатегорії, якщо вони вже існують
                return true;
            }

            if (MergeIntoSubcategories(target, sub.Subcategories))
            {
                return true;
            }
        }

        return false;
    }
}
using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BLL.Commands.CommonCommands;

public class LoadCategoriesCommand : AbstrCommandWithDA<List<CategoryModel>>
{
    public override string Name => "Завантаження категорій";

    internal LoadCategoriesCommand(IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper) { }

    public override List<CategoryModel> Execute()
    {
        var categories = dAPoint.CategoryRepository.GetQueryable()
            .Include(c => c.Subcategories)
            .ToList();

        LogAction($"Було завантажено {categories.Count} категорій");

        return mapper.Map<List<CategoryModel>>(categories);
    }
}

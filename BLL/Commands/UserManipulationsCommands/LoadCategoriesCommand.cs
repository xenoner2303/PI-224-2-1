using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;

namespace BLL.Commands.UserManipulationsCommands;

internal class LoadCategoriesCommand : AbstrCommandWithDA<List<CategoryModel>>
{
    public override string Name => "Завантаження категорій";

    internal LoadCategoriesCommand(IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper) { }

    public override List<CategoryModel> Execute()
    {
        var categories = dAPoint.CategoryRepository.GetAll();

        LogAction($"Було завантажено {categories.Count} категорій");

        return mapper.Map<List<CategoryModel>>(categories);
    }
}

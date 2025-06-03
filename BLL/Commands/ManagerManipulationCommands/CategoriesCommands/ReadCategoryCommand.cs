using AutoMapper;
using DAL.Data;
using BLL.EntityBLLModels;

namespace BLL.Commands.ManagerManipulationCommands;

internal class ReadCategoryCommand : AbstrCommandWithDA<CategoryModel?>
{
    IMapper _mapper;
    private readonly int _categoryId;

    public ReadCategoryCommand(int categoryId, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        if (categoryId <= 0)
        {
            throw new ArgumentException("Id категорії повинне бути більше 0", nameof(categoryId));
        }

        _mapper = mapper;
        _categoryId = categoryId;
    }

    public override string Name => "Отримання категорії";

    public override CategoryModel? Execute()
    {
        var category = dAPoint.CategoryRepository.GetById(_categoryId);

        if (category == null)
            return null;

        LogAction($"{Name} з ID {_categoryId}: {category.Name}");

        return category != null ? _mapper.Map<CategoryModel>(category) : null;
    }
}

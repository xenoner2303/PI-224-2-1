using AutoMapper;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class ReadCategoryCommand : AbstrCommandWithDA<Category>
    {
        private readonly int _categoryId;

        public ReadCategoryCommand(int categoryId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _categoryId = categoryId;
        }

        public override string Name => "Отримання категорії";

        public override Category Execute()
        {
            var category = dAPoint.CategoryRepository.GetById(_categoryId);

            if (category == null)
                return null;

            LogAction($"{Name} з ID {_categoryId}: {category.Name}");

            return category;
        }
    }
}

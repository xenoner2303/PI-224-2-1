using AutoMapper;
using DAL.Data;

namespace BLL.Commands.ManagerManipulationCommands
{
    internal class ReadCategoryCommand : AbstrCommandWithDA<bool>
    {
        private readonly int _categoryId;

        public ReadCategoryCommand(int categoryId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _categoryId = categoryId;
        }

        public override string Name => "Отримання категорії";

        public override bool Execute()
        {
            var category = dAPoint.CategoryRepository.GetById(_categoryId);

            LogAction($"{Name} з ID {_categoryId}: {category.Name}");
            return true;
        }
    }
}

using AutoMapper;
using BLL.Commands;
using DAL.Data;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class DeleteCategoryCommand : AbstrCommandWithDA<bool>
    {
        private readonly int _categoryId;
        public DeleteCategoryCommand(int categoryId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentException("Id категорії повинне бути більше 0", nameof(categoryId));
            }

            _categoryId = categoryId;
        }

        public override string Name => "Видалення категорії";

        public override bool Execute()
        {
            var category = dAPoint.CategoryRepository.GetById(_categoryId);
            if (category == null)
            {
                LogAction($"Не вдалося знайти категорію з ID {_categoryId}");
                return false;
            }

            dAPoint.CategoryRepository.Remove(_categoryId);
            dAPoint.Save();

            LogAction($"{Name} \"{category.Name}\" та всіх її підкатегорій");
            return true;

        }
    }
}
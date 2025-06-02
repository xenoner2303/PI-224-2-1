using AutoMapper;
using BLL.Commands;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class CreateCategoryCommand : AbstrCommandWithDA<bool>
    {
        private CategoryModel category;

        public CreateCategoryCommand(CategoryModel category, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            ArgumentNullException.ThrowIfNull(category, nameof(category));

            this.category = category;
        }

        public override string Name => "Створення нової категорії";

        public override bool Execute()
        {
            try
            {
                var newCategory = mapper.Map<Category>(category);

                dAPoint.CategoryRepository.Add(newCategory);
                dAPoint.Save();

                LogAction($"{Name} \"{category.Name}\"");
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}

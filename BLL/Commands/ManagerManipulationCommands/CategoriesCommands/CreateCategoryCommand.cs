using AutoMapper;
using BLL.Commands;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.ManagerManipulationCommands
{
    internal class CreateCategoryCommand : AbstrCommandWithDA<bool>
    {
        private readonly CategoryModel _category;

        public CreateCategoryCommand(CategoryModel category, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            ArgumentNullException.ThrowIfNull(category, nameof(category));

            // ► обрізаємо пробіли одразу, щоб і валідація, і перевірка унікальності
            //   працювали вже з “чистим” значенням
            category.Name = category.Name?.Trim();

            _category = category;

            ValidateModel();
        }

        public override string Name => "Створення нової категорії";

        public override bool Execute()
        {
            var newCategory = mapper.Map<Category>(_category);

            dAPoint.CategoryRepository.Add(newCategory);
            dAPoint.Save();

            LogAction($"{Name} \"{_category.Name}\"");
            return true;
        }

        private void ValidateModel()
        {
            // 1. Перевірка на наявність імені
            if (string.IsNullOrWhiteSpace(_category.Name))
                throw new ArgumentException("Назва категорії не може бути порожньою", nameof(_category.Name));

            // 2. Перевірка унікальності назви (вже обрізаної)
            var exists = dAPoint.CategoryRepository
                                .GetAll()
                                .Any(c => c.Name.Equals(_category.Name, StringComparison.OrdinalIgnoreCase));
            if (exists)
                throw new InvalidOperationException("Категорія з такою назвою вже існує");

            // 3. Перевірка батьківської категорії
            if (_category.ParentId.HasValue)
            {
                var parent = dAPoint.CategoryRepository.GetById(_category.ParentId.Value);
                if (parent == null)
                    throw new ArgumentException("Батьківська категорія не знайдена", nameof(_category.ParentId));
            }
        }
    }
}
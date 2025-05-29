using AutoMapper;
using DAL.Data;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class CreateCategoryCommand : AbstrCommandWithDA<bool>
    {
        private readonly string _categoryName;
        public CreateCategoryCommand(string categoryName, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _categoryName = categoryName;
        }

        public override string Name => "Ствоерення нової категорії";

        //якщо у категорії нема батька
        public override bool Execute()
        {
            try
            {
                var newCategory = new Category
                {
                    Name = _categoryName,
                    ParentId = null // категорія без батька
                };

                dAPoint.CategoryRepository.Add(newCategory);
                dAPoint.Save();
                LogAction($"{Name} \"{_categoryName}\"");
                return true;
            }
            catch(ArgumentException ex)
            {
                return false;
            }
        }

        //якщо у категорії є батько (ця категорія - підкатегорія)
        public bool Execute(int parentCategoryId)
        {
            var newCategory = new DAL.Entities.Category
            {
                Name = _categoryName,
                ParentId = parentCategoryId // категорія з батьком
            };

            var parentCategory = dAPoint.CategoryRepository.GetAll()
                .FirstOrDefault(c => c.Id == parentCategoryId);
            if (parentCategory != null) {
                parentCategory.Subcategories.Add(newCategory);
                dAPoint.CategoryRepository.Update(parentCategory);
                dAPoint.Save();
            }
            else
            {
                throw new ArgumentNullException(nameof(parentCategoryId), "Батьківська категорія не знайдена.");
            }

            dAPoint.CategoryRepository.Add(newCategory);
            dAPoint.Save();
            LogAction($"{Name} \"{_categoryName}\", яка є підкатегорією для категорії {parentCategory.Name}");
            return true;
        }
    }
}

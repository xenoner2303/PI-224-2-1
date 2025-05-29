using AutoMapper;
using DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class DeleteCategoryCommand : AbstrCommandWithDA<bool>
    {
        private readonly int _categoryId;
        public DeleteCategoryCommand(int categoryId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _categoryId = categoryId;
        }

        public override string Name => "Видалення категорії";

        public override bool Execute()
        {
            var categoryToDelete = dAPoint.CategoryRepository.GetAll()
                .FirstOrDefault(c => c.Id == _categoryId);

            var subcategoriesToDelete = dAPoint.CategoryRepository.GetAll()
                .Where(c => c.ParentId == _categoryId).ToList();
            foreach (var subcategory in subcategoriesToDelete)
            {
                dAPoint.CategoryRepository.Remove(subcategory.Id);
                dAPoint.Save();
            }
            dAPoint.CategoryRepository.Remove(_categoryId);
            dAPoint.Save();

            LogAction($"{Name}  \"{categoryToDelete.Name}\" та всіх її підкатегорій");
            return true;
        }

        //якщо у категорії є батько (ця категорія - підкатегорія)
        public bool Execute(int parentCategoryId)
        {
            var categoryToDelete = dAPoint.CategoryRepository.GetAll()
                .FirstOrDefault(c => c.Id == _categoryId);

            var parentCategory = dAPoint.CategoryRepository.GetAll()
                .FirstOrDefault(c => c.Id == parentCategoryId);
            if(parentCategory == null)
            {
                throw new ArgumentNullException(nameof(parentCategoryId), "Батьківська категорія не знайдена.");
            }
            else
            {
                parentCategory.Subcategories.Remove(categoryToDelete!);
                dAPoint.CategoryRepository.Update(parentCategory);
                dAPoint.Save();
                dAPoint.CategoryRepository.Remove(_categoryId);
                dAPoint.Save();
                LogAction($"{Name} \"{categoryToDelete.Name}\", яка є підкатегорією для категорії {parentCategory.Name}");
                return true;
            }            
        }
    }
}

using AutoMapper;
using BLL.EntityBLLModels;
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
        private readonly CategoryModel _category;
        public CreateCategoryCommand(CategoryModel category, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _category = category;
        }

        public override string Name => "Ствоерення нової категорії";

        //якщо у категорії нема батька
        public override bool Execute()
        {
            try
            {
                var newCategory = new Category
                {
                    Name = _category.Name,
                    ParentId = _category.Parent?.Id // якщо ParentId == null, то це коренева категорія
                };
                
                dAPoint.CategoryRepository.Add(newCategory);
                dAPoint.Save();
                LogAction($"{Name} \"{_category.Name}\"");
                return true;
            }
            catch(ArgumentException ex)
            {
                return false;
            }
        }
    }
}

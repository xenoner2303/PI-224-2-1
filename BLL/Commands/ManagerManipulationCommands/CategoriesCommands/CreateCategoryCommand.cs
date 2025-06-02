using AutoMapper;
using BLL.Commands;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;
using System.Linq;
using System.Xml.Linq;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class CreateCategoryCommand : AbstrCommandWithDA<bool>
    {
        private string _name;
        private int? _parentId;

        public CreateCategoryCommand(string name, IUnitOfWork unitOfWork, IMapper mapper, int? parentId = null)
            : base(unitOfWork, mapper)
        {
            _name = name;
            _parentId = parentId;
        }

        public override string Name => "Створення нової категорії";

        public override bool Execute()
        {
            try
            {
                var category = new Category
                {
                    Name = _name,
                    ParentId = _parentId
                };
                dAPoint.CategoryRepository.Add(category);
                dAPoint.Save();

                LogAction($"{Name} \"{_name}\"");
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}

using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class ManagerCommandManager : AbstractCommandManager
    {
        public ManagerCommandManager(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public bool CreateCategory(string categoryModel)
        {
            var command = new CreateCategoryCommand(categoryModel, unitOfWork, mapper);
            return ExecuteCommand(command, "Не вдалося додати категорію");
        }
        public bool DeleteCategory(int categoryId)
        {
            var command = new DeleteCategoryCommand(categoryId, unitOfWork, mapper);
            return ExecuteCommand(command, "Не вдалося видалити категорію");
        }
        public Category ReadCategory(int categoryId)
        {
            var command = new ReadCategoryCommand(categoryId, unitOfWork, mapper);
            return ExecuteCommand<Category>(command, "Не вдалося прочитати категорію");
        }
        public bool ApproveLot(int lotId)
        {
            var command = new ApproveLotCommand(lotId, unitOfWork, mapper);
            return ExecuteCommand(command, "Не вдалося підтвердити лот");
        }
        public bool RejectLot(int lotId)
        {
            var command = new RejectLotCommand(lotId, unitOfWork, mapper);
            return ExecuteCommand(command, "Не вдалося відхилити лот");
        }
        public bool StopLot(int lotId)
        {
            var command = new StopLotCommand(lotId, unitOfWork, mapper);
            return ExecuteCommand(command, "Не вдалося зупинити лот");
        }
    }
}

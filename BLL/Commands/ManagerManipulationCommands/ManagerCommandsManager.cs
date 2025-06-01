using AutoMapper;
using DAL.Data;
using BLL.Commands.CommonCommands;
using BLL.EntityBLLModels;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class ManagerCommandsManager : AbstractCommandManager
    {
        public ManagerCommandsManager(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper) { }

        public bool CreateCategory(string categoryName, int? ParentId)
        {
            var command = new CreateCategoryCommand(categoryName, unitOfWork, mapper, ParentId);
            return ExecuteCommand(command, $"Не вдалося додати категорію");
        }
        public bool DeleteCategory(int categoryId)
        {
            var command = new DeleteCategoryCommand(categoryId, unitOfWork, mapper);
            return ExecuteCommand(command, "Не вдалося видалити категорію");
        }
        public CategoryModel ReadCategory(int categoryId)
        {
            var command = new ReadCategoryCommand(categoryId, unitOfWork, mapper);
            return ExecuteCommand<CategoryModel>(command, "Не вдалося прочитати категорію");
        }
        public List<CategoryModel> LoadAllCategories()
        {
            var command = new LoadCategoriesCommand(unitOfWork, mapper);
            return ExecuteCommand<List<CategoryModel>>(command, "Не вдалося завантажити категорії");
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
        public List<AuctionLotModel> LoadAuctionLots()
        {
            var command = new LoadAuctionLotsCommand(unitOfWork, mapper);
            return ExecuteCommand(command, "Не вдалося завантажити лоти");
        }
        public List<AuctionLotModel> SearchLots(string? keyword, int? categoryId)
        {
            var command = new SearchLotsCommand(keyword, categoryId, unitOfWork, mapper);
            return ExecuteCommand<List<AuctionLotModel>>(command, "Не вдалося виконати пошук лотів");
        }
    }
}

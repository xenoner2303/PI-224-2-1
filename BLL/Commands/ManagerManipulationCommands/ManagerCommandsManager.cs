using AutoMapper;
using DAL.Data;
using DAL.Entities;
using BLL.Commands.CommonCommands;
using BLL.EntityBLLModels;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class ManagerCommandsManager : AbstractCommandManager
    {
        public ManagerCommandsManager(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper) { }

        public bool CreateCategory(CategoryModel categoryModel)
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
        public List<AuctionLotModel> LoadAuctionLots()
        {
            var command = new LoadAuctionLotsCommand(unitOfWork, mapper);
            return ExecuteCommand(command, "Не вдалося завантажити лоти");
        }
        public List<CategoryModel> LoadCategories()
        {
            var command = new LoadCategoriesCommand(unitOfWork, mapper);
            return ExecuteCommand<List<CategoryModel>>(command, "Не вдалося завантажити категорії");
        }

        public List<AuctionLot> SearchLots(string? keyword, int? categoryId)
        {
            var command = new SearchLotsCommand(keyword, categoryId, unitOfWork, mapper);
            return ExecuteCommand<List<AuctionLot>>(command, "Не вдалося виконати пошук лотів");
        }
    }
}

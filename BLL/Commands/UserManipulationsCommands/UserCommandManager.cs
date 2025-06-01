using AutoMapper;
using BLL.Commands.CommonCommands;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.UserManipulationsCommands;

public class UserCommandManager : AbstractCommandManager
{
    public UserCommandManager(IUnitOfWork unitOfWork, IMapper mapper)
    : base(unitOfWork, mapper) { }

    public List<AuctionLotModel> LoadAuctionLots()
    {
        var command = new LoadAuctionLotsCommand(unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося завантажити лоти");
    }

    public bool CreateBid(BidModel bid)
    {
        var command = new CreateBidCommand(bid, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося створити ставку");
    }

    public bool CreateLot(AuctionLotModel model)
    {
        var command = new CreateLotCommand(model, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося створити лот");
    }

    public bool DeleteLot(int lotId)
    {
        var command = new DeleteLotCommand(lotId, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося видалити лот");
    }

    public List<AuctionLotModel> LoadUserLots(int userId)
    {
        var command = new LoadUserLotsCommand(userId, unitOfWork, mapper);
        return ExecuteCommand<List<AuctionLotModel>>(command, "Не вдалося зчитати лоти користувача");
    }

    public List<CategoryModel> LoadCategories()
    {
        var command = new LoadCategoriesCommand(unitOfWork, mapper);
        return ExecuteCommand<List<CategoryModel>>(command, "Не вдалося завантажити категорії");
    }

    public List<AuctionLotModel> SearchLots(string? keyword, int? categoryId)
    {
        var command = new SearchLotsCommand(keyword, categoryId, unitOfWork, mapper);
        return ExecuteCommand<List<AuctionLotModel>>(command, "Не вдалося виконати пошук лотів");
    }
}

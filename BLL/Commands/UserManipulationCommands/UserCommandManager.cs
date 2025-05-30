using AutoMapper;
using BLL.Commands.UserManipulationsCommands.LotCommand;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.UserManipulationCommands;

public class UserCommandManager : AbstractCommandManager
{
    public UserCommandManager(IUnitOfWork unitOfWork, IMapper mapper)
    : base(unitOfWork, mapper) { }

    public bool CreateBid(decimal amount, AuctionLotModel auctionLotModel)
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

    public List<AuctionLot> LoadUserLots(int userId)
    {
        var command = new LoadUserLotsCommand(userId, unitOfWork, mapper);
        return ExecuteCommand<List<AuctionLot>>(command, "Не вдалося зчитати лоти користувача");
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

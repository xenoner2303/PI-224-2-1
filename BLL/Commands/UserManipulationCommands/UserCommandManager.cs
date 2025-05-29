using AutoMapper;
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
        var command = new CreateBidCommand(amount, auctionLotModel, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося створити ставку");
    }
    
    public bool CreateLot(string title, string description, decimal startPrice, DateTime startTime, DateTime endTime)
    {
        var command = new CreateLotCommand(title, description, startPrice, startTime, endTime, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося створити лот");
    }
    
    public bool DeleteLot(int lotId)
    {
        var command = new DeleteLotCommand(lotId, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося видалити лот");
    }
    public AuctionLot ReadLot(int lotId)
    {
        var command = new ReadLotCommand(lotId, unitOfWork, mapper);
        return ExecuteCommand<AuctionLot>(command, "Не вдалося зчитати лот");
    }

    public List<AuctionLot> ReadLots(int userId)
    {
        var command = new ReadLotsCommand(userId, unitOfWork, mapper);
        return ExecuteCommand <List<AuctionLot>>(command, "Не вдалося зчитати лот");
    }
}

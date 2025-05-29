using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;

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
}

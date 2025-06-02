using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BLL.Commands.UserManipulationsCommands;

public class LoadUserLotsCommand : AbstrCommandWithDA<List<AuctionLotModel>>
{
    private readonly int userId;
    public LoadUserLotsCommand(int userId, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        if(userId <= 0)
        {
            throw new ArgumentException("Id користувача повинне бути більше 0", nameof(userId));
        }

        this.userId = userId;
    }

    public override string Name => "Отримання лотів користувача";

    public override List<AuctionLotModel> Execute()
    {
        var usersLots = dAPoint.AuctionLotRepository.GetQueryable()
            .Include(lot => lot.Owner)
            .Include(lot => lot.Bids)
            .Where(l => l.OwnerId == userId)
            .ToList();

        LogAction($"Було завантажено {usersLots.Count} лотів користувача з Id {userId}");

        return mapper.Map<List<AuctionLotModel>>(usersLots); ;
    }
}

using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Commands.UserManipulationsCommands
{
    internal class LoadUserLotsCommand : AbstrCommandWithDA<List<AuctionLotModel>>
    {
        private readonly int userId;
        public LoadUserLotsCommand(int userId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            this.userId = userId;
        }

        public override string Name => "Отримання лотів";

        public override List<AuctionLotModel> Execute()
        {
            var usersLots = dAPoint.AuctionLotRepository.GetQueryable()
                .Include(lot => lot.Owner)
                .Include(lot => lot.Bids)
                .Where(l => l.OwnerId == userId)
                .ToList();

            LogAction($"Було завантажено {usersLots.Count} лотів користувача з айді {userId}");

            return mapper.Map<List<AuctionLotModel>>(usersLots); ;
        }
    }
}

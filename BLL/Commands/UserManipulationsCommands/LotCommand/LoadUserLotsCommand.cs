using AutoMapper;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.UserManipulationsCommands
{
    internal class LoadUserLotsCommand : AbstrCommandWithDA<List<AuctionLot>>
    {
        private readonly int userId;
        public LoadUserLotsCommand(int userId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            this.userId = userId;
        }

        public override string Name => "Отримання лотів";

        public override List<AuctionLot> Execute()
        {
            var usersLots = dAPoint.AuctionLotRepository.GetAll().Where(l => l.OwnerId == userId).ToList();

            return usersLots;
        }
    }
}

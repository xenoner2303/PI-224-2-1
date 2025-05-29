using AutoMapper;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.UserManipulationsCommands
{
    internal class LoadUserLotsCommand : AbstrCommandWithDA<List<AuctionLot>>
    {
        private readonly int _userId;
        public LoadUserLotsCommand(int userId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _userId = userId;
        }

        public override string Name => "Отримання лотів";

        public override List<AuctionLot> Execute()
        {
            var usersLots = dAPoint.AuctionLotRepository.GetAll().Where(l => l.OwnerId == _userId).ToList();

            return usersLots;
        }
    }
}

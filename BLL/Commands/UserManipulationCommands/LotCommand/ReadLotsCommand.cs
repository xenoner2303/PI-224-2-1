using AutoMapper;
using DAL.Data;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Commands.UserManipulationCommands
{
    internal class ReadLotsCommand : AbstrCommandWithDA<List<AuctionLot>>
    {
        private readonly int _userId;
        public ReadLotsCommand(int userId, IUnitOfWork unitOfWork, IMapper mapper)
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

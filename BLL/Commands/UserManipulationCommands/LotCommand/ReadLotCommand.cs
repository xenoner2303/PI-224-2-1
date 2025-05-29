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
    public class ReadLotCommand : AbstrCommandWithDA<AuctionLot>
    {
        private readonly int _lotId;
        public ReadLotCommand(int lotId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _lotId = lotId;
        }

        public override string Name => "Отримання лотів";

        public override AuctionLot Execute()
        {
            var usersLot = dAPoint.AuctionLotRepository.GetById(_lotId);

            return usersLot;
        }

    }
}

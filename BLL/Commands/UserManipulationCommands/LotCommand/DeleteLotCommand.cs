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
    internal class DeleteLotCommand : AbstrCommandWithDA<bool>
    {
        private readonly int _lotId;
        public DeleteLotCommand(int lotId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {         
            _lotId = lotId;
        }

        public override string Name => "Видалення лоту";

        public override bool Execute()
        {
            dAPoint.AuctionLotRepository.Remove(_lotId);
            dAPoint.Save();
            return true;
        }
    }
}

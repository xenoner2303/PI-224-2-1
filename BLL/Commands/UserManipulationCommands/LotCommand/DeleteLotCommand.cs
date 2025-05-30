using AutoMapper;
using DAL.Data;

namespace BLL.Commands.UserManipulationCommands
{
    internal class DeleteLotCommand : AbstrCommandWithDA<bool>
    {
        private readonly int lotId;
        public DeleteLotCommand(int lotId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {         
            this.lotId = lotId;
        }

        public override string Name => "Видалення лоту";

        public override bool Execute()
        {
            dAPoint.AuctionLotRepository.Remove(lotId);
            dAPoint.Save();
            return true;
        }
    }
}

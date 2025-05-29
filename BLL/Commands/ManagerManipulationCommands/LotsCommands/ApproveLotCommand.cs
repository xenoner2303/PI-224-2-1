using AutoMapper;
using DAL.Data;
namespace BLL.Commands.ManagerManipulationCommands
{
    public class ApproveLotCommand : AbstrCommandWithDA<bool>
    {
        private readonly int _lotId;
        public ApproveLotCommand(int lotId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _lotId = lotId;
        }

        public override string Name => "Підтвердження початку аукціону";

        public override bool Execute()
        {
            var auctionLot = dAPoint.AuctionLotRepository.GetAll()
                .FirstOrDefault(l => l.Id == _lotId);
            if (auctionLot != null)
            {
                auctionLot.Status = DAL.Entities.EnumLotStatuses.Active;
                auctionLot.StartTime = DateTime.Now;
                dAPoint.AuctionLotRepository.Update(auctionLot);
                dAPoint.Save();

                LogAction($"{Name} користувача {auctionLot.Owner.FirstName} {auctionLot.Owner.LastName} o {DateTime.Now}");
                return true;
            }
            else
            {
                throw new InvalidOperationException($"Лот з ID {_lotId} не знайдено.");
            }
        }
    }
}

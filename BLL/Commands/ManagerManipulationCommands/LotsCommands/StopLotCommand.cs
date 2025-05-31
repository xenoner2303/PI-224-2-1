using AutoMapper;
using DAL.Data;

namespace BLL.Commands.ManagerManipulationCommands
{
    internal class StopLotCommand : AbstrCommandWithDA<bool>
    {
        private readonly int _lotId;

        public StopLotCommand(int lotId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _lotId = lotId;
        }

        public override string Name => "Форсоване завершення аукціону";

        public override bool Execute()
        {
            var auctionLot = dAPoint.AuctionLotRepository.GetAll()
                .FirstOrDefault(l => l.Id == _lotId);

            if (auctionLot != null)
            {
                auctionLot.Status = DAL.Entities.EnumLotStatuses.Completed;
                auctionLot.EndTime = DateTime.Now;
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

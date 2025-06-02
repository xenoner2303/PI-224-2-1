using AutoMapper;
using DAL.Data;
using Microsoft.EntityFrameworkCore;
using BLL.EntityBLLModels;

namespace BLL.Commands.ManagerManipulationCommands
{
    public class RejectLotCommand : AbstrCommandWithDA<bool>
    {
        private readonly AuctionLotModel _lotModel;
        public RejectLotCommand(AuctionLotModel lotModel, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _lotModel = lotModel ?? throw new ArgumentNullException(nameof(lotModel), "Лот не може бути null");
        }

        public override string Name => "Відхилення лоту";

        public override bool Execute()
        {
            var auctionLot = dAPoint.AuctionLotRepository.GetById(_lotModel.Id);

            if (auctionLot != null)
            {
                auctionLot.Status = DAL.Entities.EnumLotStatuses.Rejected;
                auctionLot.EndTime = DateTime.Now;
                auctionLot.RejectionReason = _lotModel.RejectionReason;
                dAPoint.AuctionLotRepository.Update(auctionLot);
                dAPoint.Save();

                LogAction($"{Name} користувача {auctionLot.Owner.FirstName} {auctionLot.Owner.LastName} o {DateTime.Now}");
                return true;
            }
            else
            {
                throw new InvalidOperationException($"Лот не знайдено.");
            }
        }
    }
}

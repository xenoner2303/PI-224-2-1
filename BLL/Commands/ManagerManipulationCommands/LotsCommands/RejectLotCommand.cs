using AutoMapper;
using DAL.Data;
using Microsoft.EntityFrameworkCore;
using BLL.EntityBLLModels;

namespace BLL.Commands.ManagerManipulationCommands;

public class RejectLotCommand : AbstrCommandWithDA<bool>
{
    private readonly int _lotId;
    public RejectLotCommand(int lotId, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        if (lotId <= 0)
        {
            throw new ArgumentException("Id лоту повинне бути більше 0", nameof(lotId));
        }

        public override string Name => "Відхилення лоту";
        _lotId = lotId;
    }

    public override string Name => "Форсована зупинка аукціону";

        public override bool Execute()
        {
            var auctionLot = dAPoint.AuctionLotRepository.GetById(_lotModel.Id);
    public override bool Execute()
    {
        var auctionLot = dAPoint.AuctionLotRepository.GetQueryable()
            .Include(lot => lot.Owner)
            .FirstOrDefault(l => l.Id == _lotId);

            if (auctionLot != null)
            {
                auctionLot.Status = DAL.Entities.EnumLotStatuses.Rejected;
                auctionLot.EndTime = DateTime.Now;
                auctionLot.RejectionReason = _lotModel.RejectionReason;
                dAPoint.AuctionLotRepository.Update(auctionLot);
                dAPoint.Save();
        if (auctionLot != null)
        {
            auctionLot.Status = DAL.Entities.EnumLotStatuses.Rejected;
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
            LogAction($"{Name} користувача {auctionLot.Owner.FirstName} {auctionLot.Owner.LastName} o {DateTime.Now}");
            return true;
        }
        else
        {
            throw new InvalidOperationException($"Лот з ID {_lotId} не знайдено.");
        }
    }
}

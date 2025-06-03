using AutoMapper;
using DAL.Data;
using Microsoft.EntityFrameworkCore;
using BLL.EntityBLLModels;

namespace BLL.Commands.ManagerManipulationCommands;

internal class RejectLotCommand : AbstrCommandWithDA<bool>
{
    private readonly AuctionLotModel _auctionLotModel;
    public RejectLotCommand(AuctionLotModel auctionLotModel, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        _auctionLotModel = auctionLotModel ?? throw new ArgumentNullException(nameof(auctionLotModel));
    }
    public override string Name => "Відхилення лоту";

    public override bool Execute()
    {
        var auctionLot = dAPoint.AuctionLotRepository.GetById(_auctionLotModel.Id);

        if (auctionLot != null)
        {
            auctionLot.Status = DAL.Entities.EnumLotStatuses.Rejected;
            auctionLot.EndTime = DateTime.Now;
            auctionLot.RejectionReason = _auctionLotModel.RejectionReason;
            dAPoint.AuctionLotRepository.Update(auctionLot);
            dAPoint.Save();

            LogAction($"{Name} o {DateTime.Now}");
            return true;
        }
        else
        {
            throw new InvalidOperationException($"Лот не знайдено.");
        }
    }
}
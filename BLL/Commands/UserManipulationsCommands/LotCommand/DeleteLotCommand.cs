using AutoMapper;
using DAL.Data;

namespace BLL.Commands.UserManipulationsCommands;

public class DeleteLotCommand : AbstrCommandWithDA<bool>
{
    private readonly int lotId;
    public DeleteLotCommand(int lotId, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        if (lotId <= 0)
        {
            throw new ArgumentException("Id лоту повинне бути більше 0", nameof(lotId));
        }

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

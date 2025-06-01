using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.UserManipulationCommands;

public class CreateLotCommand : AbstrCommandWithDA<bool>
{
    private readonly AuctionLotModel lotModel;

    public CreateLotCommand(AuctionLotModel lotModel, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        ArgumentNullException.ThrowIfNull(lotModel, nameof(lotModel));

        this.lotModel = lotModel;
    }

    public override string Name => "Створення лоту";

    public override bool Execute()
    {
        // Мапимо Model -> Entity
        var newLot = mapper.Map<AuctionLot>(lotModel);

        dAPoint.AuctionLotRepository.Add(newLot);
        dAPoint.Save();

        return true;
    }
}

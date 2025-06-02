using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Commands.CommonCommands;

public class LoadAuctionLotsCommand : AbstrCommandWithDA<List<AuctionLotModel>>
{
    private List<EnumLotStatuses> restrictOnes;
    public override string Name => "Завантаження списку лотів";

    internal LoadAuctionLotsCommand(List<EnumLotStatuses> restrictOnes, IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper)
    { 
        if(restrictOnes == null)
        {
            this.restrictOnes = new List<EnumLotStatuses>();
        }
        else
        {
            this.restrictOnes = restrictOnes;
        }
    }

    public override List<AuctionLotModel> Execute()
    {
        var lots = dAPoint.AuctionLotRepository.GetQueryable()
            .Include(lot => lot.Owner)
            .Include(lot => lot.Bids)
            .Where(lot => !restrictOnes.Contains(lot.Status))
            .ToList();

        LogAction($"Було завантажено {lots.Count} підтверджених лотів");

        return mapper.Map<List<AuctionLotModel>>(lots);
    }
}

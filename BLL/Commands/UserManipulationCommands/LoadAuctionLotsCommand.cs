﻿using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.UserManipulationsCommands;

internal class LoadAuctionLotsCommand : AbstrCommandWithDA<List<AuctionLotModel>>
{
    public override string Name => "Завантаження списку лотів";

    internal LoadAuctionLotsCommand(IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper) { }

    public override List<AuctionLotModel> Execute()
    {
        var lots = dAPoint.AuctionLotRepository.GetAll()
            .Where(lot => lot.Status != EnumLotStatuses.Pending &&
                          lot.Status != EnumLotStatuses.Rejected)
            .ToList();

        LogAction($"Було завантажено {lots.Count} підтверджених лотів");

        return mapper.Map<List<AuctionLotModel>>(lots);
    }
}

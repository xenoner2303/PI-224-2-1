using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using System;
using DAL.Entities;

namespace BLL.Commands.UserManipulationCommands
{
    public class CreateBidCommand : AbstrCommandWithDA<bool>
    {
        private readonly IMapper _mapper;
        private readonly AuctionLotModel _lotModel;
        private readonly decimal _amount;

        public CreateBidCommand(decimal amount, AuctionLotModel auctionLotModel, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _amount = amount;
            _lotModel = auctionLotModel;
            _amount = amount;
        }

        public override string Name => "Створення ставки";

        public override bool Execute()
        {
            try
            {
                var lotEntity = dAPoint.AuctionLotRepository.GetById(_lotModel.Id);
                var newBidModel = new BidModel
                {
                    Amount = _amount,
                    PlacedAt = DateTime.Now,
                    Lot = _lotModel,
                    User = _lotModel.Owner
                };
                var newBidEntity = _mapper.Map<Bid>(newBidModel);
                int countBids = lotEntity.Bids.Count();
                if (countBids >= 1)
                {
                    if (lotEntity.Bids[countBids - 1].Amount < newBidEntity.Amount)
                    {
                        lotEntity.Bids.Add(newBidEntity);
                        dAPoint.AuctionLotRepository.Update(lotEntity);
                        dAPoint.Save();
                        LogAction($"{Name} на суму {newBidEntity.Amount} користувачем {_lotModel.Owner.FirstName} {_lotModel.Owner.LastName}");


                        return true;
                    }
                    LogAction($"{Name} на суму {newBidEntity.Amount} користувачем {_lotModel.Owner.FirstName} {_lotModel.Owner.LastName} НЕ ВДАЛОСЬ, бо сума ставуи була меншою за попередню");
                    return false;
                }
                else if (countBids == 0)
                {
                    lotEntity.Bids.Add(newBidEntity);
                    dAPoint.AuctionLotRepository.Update(lotEntity);
                    dAPoint.Save();
                    LogAction($"{Name} на суму {newBidEntity.Amount} користувачем {_lotModel.Owner.FirstName} {_lotModel.Owner.LastName}");

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}


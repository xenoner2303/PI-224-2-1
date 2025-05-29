using AutoMapper;
using DAL.Data;
using DAL.Entities;
using BLL.EntityBLLModels;

namespace BLL.Commands.UserManipulationsCommands
{
    public class CreateBidCommand : AbstrCommandWithDA<bool>
    {
        private readonly BidModel bidModel;

        public CreateBidCommand(BidModel bidModel, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            ArgumentNullException.ThrowIfNull(bidModel, nameof(bidModel));

            this.bidModel = bidModel;
        }

        public override string Name => "Створення ставки";

        public override bool Execute()
        {
            // Мапимо Model -> Entity
            var newBid = mapper.Map<Bid>(bidModel);

            var lot = dAPoint.AuctionLotRepository.GetById(bidModel.Lot.Id);
            lot.Bids.Add(newBid);
            dAPoint.AuctionLotRepository.Update(lot);
            dAPoint.Save();

            LogAction($"{Name} на суму {bidModel.Amount} користувачаем {bidModel.User}");
            return true;
        }
    }
}

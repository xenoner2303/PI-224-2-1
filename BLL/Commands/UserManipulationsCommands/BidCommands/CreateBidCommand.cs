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
            // мапимо Model -> Entity
            var newBid = mapper.Map<Bid>(bidModel);

            // перевіряємо чи існують об'єкти в БД
            var existingUser = dAPoint.RegisteredUserRepository.GetById(bidModel.User.Id);
            var existingLot = dAPoint.AuctionLotRepository.GetById(bidModel.Lot.Id);

            ValidateModel(existingUser, existingLot);

            newBid.Lot = existingLot;
            newBid.LotId = existingLot.Id;

            newBid.User = existingUser;
            newBid.UserId = existingUser.Id;

            existingLot.Bids.Add(newBid);
            dAPoint.AuctionLotRepository.Update(existingLot);
            dAPoint.Save();

            LogAction($"{Name} на суму {bidModel.Amount} користувачаем {bidModel.User}");
            return true;
        }

        private void ValidateModel(AbstractUser existingUser, AuctionLot existingLot)
        {
            // якщо не існує — додаємо
            if (existingUser == null)
            {
                throw new ArgumentNullException("Виконавець не співпадає з бд");
            }

            // категорія не критична - додаємо якщо в бд нема
            if (existingLot == null)
            {
                throw new ArgumentNullException("Лот не співпадає з бд");
            }
        }
    }
}

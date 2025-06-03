using AutoMapper;
using DAL.Data;
using DAL.Entities;
using BLL.EntityBLLModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Commands.UserManipulationsCommands;

internal class CreateBidCommand : AbstrCommandWithDA<bool>
{
    private readonly BidModel bidModel;

    public CreateBidCommand(BidModel bidModel, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        ArgumentNullException.ThrowIfNull(bidModel, nameof(bidModel));

        this.bidModel = bidModel;

        preValidateModel();
    }

    public override string Name => "Створення ставки";

    public override bool Execute()
    {
        // мапимо Model -> Entity
        var newBid = mapper.Map<Bid>(bidModel);

        // перевіряємо чи існують об'єкти в БД
        var existingUser = dAPoint.RegisteredUserRepository.GetById(bidModel.User.Id);
        var existingLot = dAPoint.AuctionLotRepository.GetQueryable()
            .Include(lot => lot.Bids)
            .FirstOrDefault(l => l.Id == bidModel.Lot.Id);

        ValidateModel(existingUser, existingLot);

        newBid.Lot = existingLot;
        newBid.LotId = existingLot.Id;

        newBid.User = existingUser;
        newBid.UserId = existingUser.Id;
  
        dAPoint.BidRepository.Add(newBid);        
        dAPoint.Save();

        LogAction($"{Name} на суму {bidModel.Amount} користувачаем {bidModel.User}");
        return true;
    }

    private void preValidateModel()
    {
        ArgumentNullException.ThrowIfNull(bidModel.User, "Виконавець не може бути пустим");
        ArgumentNullException.ThrowIfNull(bidModel.Lot, "Лот не може бути пустим");
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

        if (bidModel.Amount <= existingLot.StartPrice)
        {
            throw new ArgumentOutOfRangeException(nameof(bidModel.Amount), "Сума ставки не може бути меншою за мінімальну ціну");
        }

        if (existingLot.Bids.Count > 0)
        {
            if (bidModel.Amount <= existingLot.Bids.Max(b => b.Amount))
            {
                throw new ArgumentOutOfRangeException(nameof(bidModel.Amount), "Сума ставки не може бути меншою за попередню ставку");
            }
        }
    }
}

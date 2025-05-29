using AutoMapper;
using DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace BLL.Commands.UserManipulationCommands
{
    public class CreateBidCommand : AbstrCommandWithDA<bool>
    {
        private readonly decimal _amount;
        private readonly int _lotId;
        private readonly int _userId;

        public CreateBidCommand(decimal amount, int lotId, int userId, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _amount = amount;
            _lotId = lotId;
            _userId = userId;
        }

        public override string Name => "Створення ставки";

        public override bool Execute()
        {
            try
            {
                var newBid = new Bid
                {
                    Amount = _amount,
                    PlacedAt = DateTime.Now,
                    LotId = _lotId,
                    UserId = _userId
                };
                var lot = dAPoint.AuctionLotRepository.GetById(_lotId);
                lot.Bids.Add(newBid);
                dAPoint.AuctionLotRepository.Update(lot);
                var user = dAPoint.UserRepository.GetById(_userId);
                dAPoint.Save();
                LogAction($"{Name} на суму {_amount} користувачаем {user.FirstName} {user.LastName}");
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}

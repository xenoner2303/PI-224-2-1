using AutoMapper;
using DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Commands.UserManipulationsCommands
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

        public override string Name => "Видалення лоту";

        public override bool Execute()
        {
            try
            {
                var newBid = new DAL.Entities.Bid
                {
                    Amount = _amount,
                    PlacedAt = DateTime.Now,
                    LotId = _lotId,
                    UserId = _userId
                };
                dAPoint.BidRepository.Add(newBid);
                dAPoint.Save();
                //ТРЕБА ДОПИСАТИ, ЩОБ СТАВКА ЗБЕРІГАЛАСЬ І В ЛОТІ І В ЮЗЕРА
                LogAction($"{Name} на суму {_amount}");
                return true;
            }
            catch (ArgumentException ex)
            {
                return false;
            }
        }
    }
}

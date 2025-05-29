using AutoMapper;
using BLL.EntityBLLModels;
using BLL.Services;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.UserManipulationsCommands
{
    public class CreateLotCommand : AbstrCommandWithDA<bool>
    {
        private AuctionLot _auctionLot;
        public CreateLotCommand(AuctionLot lotDto, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _auctionLot = lotDto;
        }

        public override string Name => "Створення лоту";

        public override bool Execute()
        {
            dAPoint.AuctionLotRepository.Add(_auctionLot);
            dAPoint.Save();
            return true;
        }
    }
}

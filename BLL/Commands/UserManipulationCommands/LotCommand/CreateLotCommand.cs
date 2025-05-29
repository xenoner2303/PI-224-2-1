using AutoMapper;
using BLL.EntityBLLModels;
using BLL.Services;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.UserManipulationCommands
{
    public class CreateLotCommand : AbstrCommandWithDA<bool>
    {
        private readonly string _title;
        private readonly string _description;
        private readonly decimal _startPrice;
        private readonly DateTime _startTime;
        private readonly DateTime _endTime;

        public CreateLotCommand(string title, string description, decimal startPrice, DateTime startTime, DateTime endTime, IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _title = title;
            _description = description;
            _startPrice = startPrice;
            _startTime = startTime;
            _endTime = endTime;
        }

        public override string Name => "Створення лоту";

        public override bool Execute()
        {
            var newLot = new AuctionLot
            {
                Title = _title,
                Description = _description,
                StartPrice = _startPrice,
                StartTime = _startTime,
                EndTime = _endTime
            };

            dAPoint.AuctionLotRepository.Add(newLot);
            dAPoint.Save();
            return true;
        }
    }
}

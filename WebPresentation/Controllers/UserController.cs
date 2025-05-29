using AutoMapper;
using BLL.Commands.UserManipulationCommands;
using BLL.EntityBLLModels;
using DTOsLibrary;
using Microsoft.AspNetCore.Mvc;

namespace WebPresentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserCommandManager _manager;
        private readonly IMapper _mapper;
        public UserController(UserCommandManager manager, IMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(manager);
            ArgumentNullException.ThrowIfNull(mapper);

            _manager = manager;
            _mapper = mapper;
        }
        [HttpPost("create-bid")]
        public IActionResult CreateBid([FromBody] decimal amount, [FromBody] AuctionLotModel auctionLotModel)
        {
            var result = _manager.CreateBid(amount, auctionLotModel);
            if (result)
            {
                return Ok("Bid created successfully");
            }
            return BadRequest("Failed to create bid.");
        }
        [HttpPost("create-lot")]
        public IActionResult CreateLot([FromBody] string title, [FromBody] string description, [FromBody] decimal startPrice, [FromBody] DateTime startTime, DateTime endTime)
        {
            var result = _manager.CreateLot(title, description, startPrice, startTime, endTime);
            if (result)
            {
                return Ok("Lot created successfully");
            }
            else
            {
                return BadRequest("Failed to create lot");
            }
        }
        [HttpDelete("delete-lot")]
        public IActionResult DeleteLot([FromBody] int lotId)
        {
            var result = _manager.DeleteLot(lotId);
            if(result)
            {
                return Ok("Lot deleted successfully");
            }
            else
            {
                return BadRequest("Failed to delete lot");
            }
        }
        [HttpGet("read-lots")] 
        public IActionResult ReadLots([FromBody] int userId)
        {
            var result = _manager.ReadLots(userId);
            if(result != null)
            {
                return Ok("Lots successfully read");
            }
            else
            {
                return BadRequest("Failed to read lots");
            }
        }
    }
}

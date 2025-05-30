using AutoMapper;
using BLL.Commands.UserManipulationCommands;
using BLL.EntityBLLModels;
using DTOsLibrary;
using Microsoft.AspNetCore.Mvc;
using BLL.EntityBLLModels;

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

        // POST: api/User/lot
        [HttpPost("lot")]
        public ActionResult CreateLot([FromBody] AuctionLotDto dto)
        {
            try
            {
                var lot = mapper.Map<AuctionLotModel>(dto);

                var success = manager.CreateLot(lot);
                return success ? Ok() : BadRequest("Лот не створено");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/User/bid
        [HttpPost("bid")]
        public ActionResult CreateBid([FromBody] BidDto dto)
        {
            try
            {
                var bid = mapper.Map<BidModel>(dto);

                var success = manager.CreateBid(bid);
                return success ? Ok() : BadRequest("Лот не створено");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/User/lot/5
        [HttpDelete("lot/{id}")]
        public ActionResult DeleteLot(int id)
        {
            try
            {
                var success = manager.DeleteLot(id);
                return success ? Ok() : BadRequest("Лот не видалено");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/User/mylots/7
        [HttpGet("mylots/{userId}")]
        public ActionResult<List<AuctionLotDto>> GetUserLots(int userId)
        {
            try
            {
                var models = manager.LoadUserLots(userId);
                var dtos = mapper.Map<List<AuctionLotDto>>(models);
                return dtos;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/User/categories
        [HttpGet("categories")]
        public ActionResult<List<CategoryDto>> GetCategories()
        {
            try
            {
                var models = manager.LoadCategories();
                var dtos = mapper.Map<List<CategoryDto>>(models);
                return dtos;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/User/search
        [HttpPost("search")]
        public ActionResult<List<AuctionLotDto>> SearchLots([FromBody] SearchLotsDto dto)
        {
            try
            {
                var lots = manager.SearchLots(dto.Keyword, dto.CategoryId);
                var dtos = mapper.Map<List<AuctionLotDto>>(lots);
                return dtos;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using AutoMapper;
using BLL.Commands.UserManipulationsCommands;
using DTOsLibrary;
using Microsoft.AspNetCore.Mvc;
using BLL.EntityBLLModels;

namespace WebPresentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserCommandManager manager;
        private readonly IMapper mapper;

        public UserController(UserCommandManager manager, IMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(manager, nameof(manager));
            ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

            this.manager = manager;
            this.mapper = mapper;
        }

        // GET: api/User/lots
        [HttpGet("lots")]
        public ActionResult<List<AuctionLotDto>> GetAuctionLots()
        {
            try
            {
                var models = manager.LoadAuctionLots();
                var dtos = mapper.Map<List<AuctionLotDto>>(models);

                return dtos;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
    }
}

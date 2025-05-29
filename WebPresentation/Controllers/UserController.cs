using AutoMapper;
using BLL.Commands.UserManipulationsCommands;
using DTOsLibrary;
using Microsoft.AspNetCore.Mvc;

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
    }
}

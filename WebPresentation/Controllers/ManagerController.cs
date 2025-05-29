using AutoMapper;
using BLL.Commands.ManagerManipulationCommands;
using BLL.EntityBLLModels;
using DTOsLibrary;
using Microsoft.AspNetCore.Mvc;

namespace WebPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManagerController : ControllerBase
{
    private readonly ManagerCommandsManager _manager;
    private readonly IMapper _mapper;

    public ManagerController(ManagerCommandsManager manager, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(manager, nameof(manager));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

        this._manager = manager;
        this._mapper = mapper;
    }

    [HttpPost("create-category")]
    public IActionResult CreateCategory([FromBody]string categoryName)
    {
        var result = _manager.CreateCategory(categoryName);

        if (result)
        {
            return Ok("Category created successfully");
        }
        return BadRequest("Failed to create category");
    }

    [HttpDelete("delete-category")]
    public IActionResult DeleteCategory([FromBody]int categoryId)
    {
        var result = _manager.DeleteCategory(categoryId);

        if (result)
        {
            return Ok("Category deleted successfully");
        }
        return BadRequest("Failed to delete category");
    }

    [HttpGet("read-category")]
    public ActionResult<CategoryDto> ReadCategory([FromQuery]int categoryId)
    {
        var category = _manager.ReadCategory(categoryId);
        var result = _mapper.Map<CategoryDto>(category);
        if (result != null)
        {
            return Ok("Category read successfully");
        }
        return BadRequest("Failed to read category");
    }

    [HttpPost("approve-lot")]
    public IActionResult ApproveLot([FromBody]int lotId)
    {
        var result = _manager.ApproveLot(lotId);

        if (result)
        {
            return Ok("Lot approved successfully");
        }
        return BadRequest("Failed to approve lot");
    }

    [HttpPost("reject-lot")]
    public IActionResult RejectLot([FromBody]int lotId)
    {
        var result = _manager.RejectLot(lotId);

        if (result)
        {
            return Ok("Lot rejected successfully");
        }
        return BadRequest("Failed to reject lot");
    }

    [HttpPost("stop-lot")]
    public IActionResult StopLot([FromBody]int lotId)
    {
        var result = _manager.StopLot(lotId);

        if (result)
        {
            return Ok("Lot stopped successfully");
        }
        return BadRequest("Failed to stop lot");
    }
}

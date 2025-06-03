using AutoMapper;
using BLL.Commands.ManagerManipulationCommands;
using BLL.EntityBLLModels;
using DAL.Entities;
using DTOsLibrary;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace WebPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManagerController : ControllerBase
{
    private readonly ManagerCommandManager _manager;
    private readonly IMapper _mapper;

    public ManagerController(ManagerCommandManager manager, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(mapper);

        this._manager = manager;
        this._mapper = mapper;
    }

    [HttpPost("category")]
    public IActionResult CreateCategory([FromBody] CategoryDto categoryDto)
    {
        var dto = _mapper.Map<CategoryModel>(categoryDto);
        var result = _manager.CreateCategory(dto);

        if (result)
        {
            return Ok("Category created successfully");
        }
        return BadRequest("Failed to create category");
    }

    [HttpDelete("category/{categoryId}")]
    public IActionResult DeleteCategory(int categoryId)
    {
        try
        {
            var result = _manager.DeleteCategory(categoryId);

            if (result)
            {
                return Ok("Category deleted successfully");
            }
            return BadRequest("Failed to delete category");
        }
        catch(Exception)
        {
           return BadRequest("You can`t delete category if any lot with this category has been created.");
        }
    }

    [HttpGet("category/{categoryId}")]
    public ActionResult<CategoryDto> GetCategory(int categoryId)
    {
        var result = _manager.ReadCategory(categoryId);
        if (result != null)
        {
            var dto = _mapper.Map<CategoryDto>(result);
            return Ok(dto);
        }
        return NotFound("Category not found");
    }

    [HttpGet("categories")]
    public ActionResult<List<CategoryDto>> LoadAllCategories()
    {
        try
        {
            var models = _manager.LoadAllCategories();
            var dtos = _mapper.Map<List<CategoryDto>>(models);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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
    public IActionResult RejectLot([FromBody]AuctionLotDto auctionLotDto)
    {

        var auctionLotModel = _mapper.Map<AuctionLotModel>(auctionLotDto);

        var result = _manager.RejectLot(auctionLotModel);
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

    [HttpGet("lots")]
    public ActionResult<List<AuctionLotDto>> GetAuctionLots()
    {
        try
        {
            var models = _manager.LoadAuctionLots();
            var dtos = _mapper.Map<List<AuctionLotDto>>(models);

            return dtos;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("search")]
    public ActionResult<List<AuctionLotDto>> SearchLots([FromBody] SearchLotsDto dto)
    {
        try
        {
            var lots = _manager.SearchLots(dto.Keyword, dto.CategoryId);
            var dtos = _mapper.Map<List<AuctionLotDto>>(lots);
            return dtos;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

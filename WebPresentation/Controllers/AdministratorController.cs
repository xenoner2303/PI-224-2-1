using AutoMapper;
using BLL.Commands;
using BLL.EntityBLLModels;
using DTOsLibrary;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AdministratorController : ControllerBase
{
    private readonly AdministratorCommandsManager _commandsManager;
    private readonly IMapper _mapper;

    public AdministratorController(AdministratorCommandsManager manager, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(mapper);

        this._commandsManager = manager;
        this._mapper = mapper;
    }

    #region Users Endpoints
    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        try
        {
            var users = _commandsManager.LoadUsers();
            var dtos = _mapper.Map<List<BaseUserDto>>(users);

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка при отриманні користувачів: {ex.Message}");
        }
    }

    [HttpPut("users")]
    public IActionResult UpdateUsers([FromBody] List<BaseUserModel> users)
    {
        try
        {
            var result = _commandsManager.SaveUsersChanges(users);
            return result ? Ok() : BadRequest("Не вдалося оновити користувачів");
        }
        catch (Exception ex)
        {
            return BadRequest($"Помилка: {ex.Message}");
        }
    }

    [HttpDelete("users/{id}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            var result = _commandsManager.RemoveUser(id);
            return result ? Ok() : BadRequest("Не вдалося видалити користувача");
        }
        catch (Exception ex)
        {
            return BadRequest($"Помилка: {ex.Message}");
        }
    }

    [HttpGet("lots")]
    public ActionResult<List<AuctionLotDto>> GetAuctionLots()
    {
        try
        {
            var models = _commandsManager.LoadAuctionLots();
            var dtos = _mapper.Map<List<AuctionLotDto>>(models);

            return dtos;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region SecretCodeRealizators Endpoints
    [HttpGet("secretcoderealizators")]
    public IActionResult GetAllSecretCodeRealizators()
    {
        try
        {
            var realizators = _commandsManager.LoadSecretCodeRealizators();
            var dtos = _mapper.Map<List<SecretCodeRealizatorDto>>(realizators);

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка при отриманні реалізаторів: {ex.Message}");
        }
    }

    [HttpPost("secretcoderealizators")]
    public IActionResult CreateSecretCodeRealizator([FromBody] SecretCodeRealizatorModel realizatorModel)
    {
        try
        {
            var result = _commandsManager.CreateCodeRealizator(realizatorModel);
            return result ? Ok() : BadRequest("Не вдалося створити реалізатора");
        }
        catch (Exception ex)
        {
            return BadRequest($"Помилка: {ex.Message}");
        }
    }

    [HttpDelete("secretcoderealizators/{id}")]
    public IActionResult DeleteSecretCodeRealizator(int id)
    {
        try
        {
            var result = _commandsManager.RemoveSecretCodeRealizator(id);
            return result ? Ok() : BadRequest("Не вдалося видалити реалізатора");
        }
        catch (Exception ex)
        {
            return BadRequest($"Помилка: {ex.Message}");
        }
    }
    #endregion

    #region Logs Endpoints
    [HttpGet("logs/{date?}")] // у гет за стандартом нема тіла, тому параметр  ми будемо отримувати через маршрут
    public ActionResult<List<ActionLogDto>> GetLogs(DateTime? date = null)
    {
        try
        {
            var logs = _commandsManager.LoadLogs(date);
            var dtos = _mapper.Map<List<ActionLogDto>>(logs);

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion
}
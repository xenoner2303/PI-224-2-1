using AutoMapper;
using BLL.Commands;
using BLL.EntityBLLModels;
using DAL.Entities;
using DTOsLibrary;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AdministratorController : ControllerBase
{
    private readonly AdministratorCommandsManager _commandsManager;

    public AdministratorController(AdministratorCommandsManager commandsManager)
    {
        _commandsManager = commandsManager;
    }

    #region Users Endpoints
    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        try
        {
            var users = _commandsManager.LoadUsers();
            return Ok(users);
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
    #endregion

    #region SecretCodeRealizators Endpoints
    [HttpGet("secretcoderealizators")]
    public IActionResult GetAllSecretCodeRealizators()
    {
        try
        {
            var realizators = _commandsManager.LoadSecretCodeRealizators();
            return Ok(realizators);
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
            return Ok(logs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion
}
using BLL.Commands;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly AdministratorCommandsManager _commandsManager;

    public LogsController(AdministratorCommandsManager commandsManager)
    {
        _commandsManager = commandsManager;
    }

    [HttpGet]
    public IActionResult GetLogs([FromQuery] DateTime? date)
    {
        try
        {
            var logs = _commandsManager.LoadLogs(date);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Помилка при отриманні логів: {ex.Message}");
        }
    }
}
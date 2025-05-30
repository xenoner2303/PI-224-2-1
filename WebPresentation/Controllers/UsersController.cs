using BLL.Commands;
using BLL.EntityBLLModels;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AdministratorCommandsManager _commandsManager;

    public UsersController(AdministratorCommandsManager commandsManager)
    {
        _commandsManager = commandsManager;
    }

    [HttpGet]
    public IActionResult GetAll()
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

    [HttpPut]
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

    //[HttpDelete("{id}")]
    //public IActionResult Delete(int id)
    //{
    //    try
    //    {
    //        // Припускаємо, що у є метод для отримання користувача за ID
    //        var user = /* Отримати користувача за ID */;
    //        var result = _commandsManager.RemoveUser(user);
    //        return result ? Ok() : BadRequest("Не вдалося видалити користувача");
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest($"Помилка: {ex.Message}");
    //    }
    //}
}
//using BLL.Commands;
//using BLL.EntityBLLModels;
//using Microsoft.AspNetCore.Mvc;

//[ApiController]
//[Route("api/[controller]")]
//public class SecretCodeRealizatorsController : ControllerBase
//{
//    private readonly AdministratorCommandsManager _commandsManager;

//    public SecretCodeRealizatorsController(AdministratorCommandsManager commandsManager)
//    {
//        _commandsManager = commandsManager;
//    }

//    [HttpGet]
//    public IActionResult GetAll()
//    {
//        try
//        {
//            var realizators = _commandsManager.LoadSecretCodeRealizators();
//            return Ok(realizators);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, $"Помилка при отриманні реалізаторів: {ex.Message}");
//        }
//    }

//    [HttpPost]
//    public IActionResult Create([FromBody] SecretCodeRealizatorModel realizatorModel)
//    {
//        try
//        {
//            var result = _commandsManager.CreateCodeRealizator(realizatorModel);
//            return result ? Ok() : BadRequest("Не вдалося створити реалізатора");
//        }
//        catch (Exception ex)
//        {
//            return BadRequest($"Помилка: {ex.Message}");
//        }
//    }

//    [HttpDelete("{id}")]
//    public IActionResult Delete(int id)
//    {
//        try
//        {
//            // Припускаємо, що у є метод для отримання реалізатора за ID
//            var realizator = /* Отримати реалізатора за ID */;
//            var result = _commandsManager.RemoveSecretCodeRealizator(realizator);
//            return result ? Ok() : BadRequest("Не вдалося видалити реалізатора");
//        }
//        catch (Exception ex)
//        {
//            return BadRequest($"Помилка: {ex.Message}");
//        }
//    }
//}
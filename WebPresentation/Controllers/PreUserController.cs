using AutoMapper;
using BLL.Commands.PreUsersManipulationCommands;
using BLL.EntityBLLModels;
using DTOsLibrary;
using Microsoft.AspNetCore.Mvc;

namespace WebPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreUserController : ControllerBase
{
    private readonly PreUserCommandsManager manager;
    private readonly IMapper mapper;

    public PreUserController(PreUserCommandsManager manager, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(manager, nameof(manager));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

        this.manager = manager;
        this.mapper = mapper;
    }

    // Post: api/PreUser/register
    [HttpPost("register")]
    public IActionResult Register([FromBody] BaseUserDto userDto)
    {
        try
        {
            var userModel = mapper.Map<BaseUserModel>(userDto);
            var result = manager.CreateUser(userModel);
            return result ? Ok() : BadRequest("Не вдалося створити користувача");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Post: api/PreUser/authorizate
    [HttpPost("authorizate")]
    public ActionResult<BaseUserDto> Authorizate([FromBody] AuthorizationDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.login) || string.IsNullOrWhiteSpace(loginDto.password))
        {
            return BadRequest("Логін і пароль обов’язкові");
        }

        try
        {
            var user = manager.AuthorizeUser(loginDto.login, loginDto.password);

            var userDto = mapper.Map<BaseUserDto>(user);
            return userDto;
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message); // "Невірний логін або пароль"
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

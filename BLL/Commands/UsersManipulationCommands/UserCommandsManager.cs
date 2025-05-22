using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;

namespace BLL.Commands.UsersManipulationCommands;

public class UserCommandsManager : AbstractCommandManager
{
    public UserCommandsManager(IUnitOfWork unitOfWork, IMapper mapper)
    : base(unitOfWork, mapper) { }

    public bool CreateUser(BaseUserModel userModel)
    {
        var command = new CreateUserCommand(userModel, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося створити користувача");
    }

    public BaseUserModel AuthorizeUser(string login, string password)
    {
        var command = new AuthorizeUserCommand(login, password, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося авторизувати користувача");
    }
}

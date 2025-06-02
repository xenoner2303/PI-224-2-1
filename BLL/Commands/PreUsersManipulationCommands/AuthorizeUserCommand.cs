using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using BLL.Services;

namespace BLL.Commands.PreUsersManipulationCommands;

public class AuthorizeUserCommand : AbstrCommandWithDA<BaseUserModel>
{
    private readonly string login;
    private readonly string password;

    public override string Name => "Створення користувача";

    internal AuthorizeUserCommand(string login, string password, IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(login, nameof(login));
        ArgumentNullException.ThrowIfNullOrEmpty(password, nameof(password));

        this.login = login;
        this.password = password;
    }

    public override BaseUserModel Execute()
    {
        ManagerSecretCodeRealizator ds = new ManagerSecretCodeRealizator(PasswordHasher.HashPassword("qwerty"), 99);
        dAPoint.SecretCodeRealizatorRepository.Add(ds);
        dAPoint.Save();
        var matchedUser = dAPoint.UserRepository
            .GetQueryable()
            .ToList() // переносимо на клієнську сторону, бо в sql запит наш код транслюватися не може
            .FirstOrDefault(u => u.Login == login && PasswordHasher.VerifyPassword(password, u.PasswordHash));

        if (matchedUser == null)
        {
            LogAction($"Відбулася неуспішна спроба авторизації аккаунта з логіном {login}");
            throw new UnauthorizedAccessException("Невірний логін або пароль");
        }

        LogAction($"Успішна авторизація користувача типа {matchedUser.InterfaceType.ToString()} з логіном {login}");
        return mapper.Map<BaseUserModel>(matchedUser);
    }
}
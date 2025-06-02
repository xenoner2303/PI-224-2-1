using AutoMapper;
using DAL.Data;
using BLL.EntityBLLModels;
using DAL.Entities;
using BLL.Services;
using System.Text.RegularExpressions;

namespace BLL.Commands.PreUsersManipulationCommands;

public class CreateUserCommand : AbstrCommandWithDA<bool>
{
    private readonly BaseUserModel userModel;

    public override string Name => "Створення користувача";

    internal CreateUserCommand(BaseUserModel userModel, IUnitOfWork operateUnitOfWork, IMapper mapper)
        :base(operateUnitOfWork, mapper)
    {
        ArgumentNullException.ThrowIfNull(userModel, nameof(userModel));

        this.userModel = userModel;
        ValidateModel();
    }

    public override bool Execute()
    {
        // знаходимо реалізатор секретного коду
        var secretCodeRealizator = dAPoint.SecretCodeRealizatorRepository
            .GetQueryable()
            .ToList() // переносимо на клієнську сторону, бо в sql запит наш код транслюватися не може
            .FirstOrDefault(r => PasswordHasher.VerifyPassword(userModel.SecretCode, r.SecretCodeHash));

        // отримуємо білдер
        var userBuilder = secretCodeRealizator?.ReturnTypeBuilder() ?? new UserBuilder<RegisteredUser>();

        // налаштовуємо білдер
        userBuilder.SetUserFirstName(userModel.FirstName);
        userBuilder.SetUserLastName(userModel.LastName);
        userBuilder.SetUserLogin(userModel.Login);

        if (!string.IsNullOrWhiteSpace(userModel.Email))
        {
            userBuilder.SetUserEmail(userModel.Email);
        }

        if (!string.IsNullOrWhiteSpace(userModel.PhoneNumber))
        {
            userBuilder.SetPhoneNumber(userModel.PhoneNumber);
        }

        userBuilder.SetUserAge(userModel.Age);
        userBuilder.SetPassword(PasswordHasher.HashPassword(userModel.Password));

        // створюємо користувача
        var user = userBuilder.GetBuild();

        // зберігаємо його
        dAPoint.UserRepository.Add(user);
        dAPoint.Save();

        LogAction($"Користувач '{userModel.Login}' був доданий");
        return true;
    }

    private void ValidateModel()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(userModel.FirstName))
        {
            errors.Add("Ім'я користувача обов'язкове і не може бути порожнім");
        }

        if (string.IsNullOrWhiteSpace(userModel.LastName))
        {
            errors.Add("Прізвище користувача обов'язкове і не може бути порожнім");
        }

        if (string.IsNullOrWhiteSpace(userModel.Login))
        {
            errors.Add("Логін користувача обов'язковий і не може бути порожнім");
        }
        else
        {
            // перевірка на унікальність логіну
            var existingUser = dAPoint.UserRepository
                .GetQueryable()
                .FirstOrDefault(u => u.Login == userModel.Login);

            if (existingUser != null)
            {
                errors.Add($"Користувач з логіном '{userModel.Login}' уже існує");
            }
        }

        if (!string.IsNullOrWhiteSpace(userModel.Email) && !Regex.IsMatch(userModel.Email, @"^\S+@\S+\.\S+$"))
        {
            errors.Add("Невірний формат електронної пошти");
        }

        if (!string.IsNullOrWhiteSpace(userModel.PhoneNumber) && !Regex.IsMatch(userModel.PhoneNumber, @"^\+?\d{10,15}$"))
        {
            errors.Add("Невірний формат номеру телефону");
        }

        if (userModel.Age <= 0)
        {
            errors.Add("Вік користувача повинен бути більше 0");
        }

        if (string.IsNullOrWhiteSpace(userModel.Password) || userModel.Password.Length < 6)
        {
            errors.Add("Пароль повинен містити щонайменше 6 символів");
        }

        if (errors.Any())
        {
            throw new ArgumentException($"Помилки валідації моделі: {string.Join("; ", errors)}");
        }
    }
}
